using StringEnhancer;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Caphyon.RcStrings.VsPackage
{
  public class StringResourceContext
  {
    #region Members

    // Initializing
    private Encoding mCodePage;

    private HeaderContentBuilder mHeaderContentBuilder;
    private RCFileContentBuilder mRCFileContentBuilder;

    private HeaderContent mHeaderContent;
    private RCFileContent mRCFileContent;

    private IDGenerator mIDGenerator;

    private HeaderWriter mHeaderWriter;
    private RCFileWriter mRCFileWriter;

    private string mWriteRCPath;
    private string mWriteHeaderPath;

    private bool mShowGhostFile;
    //private string kDefaultResourceHeaderFileName = "resource.h";

    #endregion

    #region Properties

    public RcFile RcFile { get; private set; }
    public int GetId => mIDGenerator.Generate();
    public HeaderContent HeaderContent { get; private set; }
    private string DefaultHeaderFile { get; }
    #endregion

    #region Ctor
    /// <summary>
    /// Automatically loads the data from the RC file
    /// </summary>
    /// <param name="aRcFile"></param>
    /// <param name="aShowGhostFile"></param>
    public StringResourceContext(RcFile aRcFile, bool aShowGhostFile)
    {
      RcFile = aRcFile;
      mShowGhostFile = aShowGhostFile;

      var rcFilePath = RcFile.FilePath;
      mCodePage = CodePageExtractor.GetCodePage(rcFilePath);

      mHeaderContentBuilder = new HeaderContentBuilder(rcFilePath, mCodePage);
      mHeaderContentBuilder.Build();
      mHeaderContent = mHeaderContentBuilder.GetResult();

      HeaderContent = mHeaderContent;

      mRCFileContentBuilder = new RCFileContentBuilder(rcFilePath, mCodePage, mHeaderContent);
      mRCFileContentBuilder.Build();
      mRCFileContent = mRCFileContentBuilder.GetResult();

      var headerNames = HeaderNamesExtractor.ExtractHeaderNames(RcFile.FilePath, mCodePage);

      if (!headerNames.Any())
        return;

      var rcFileDirectory = Path.GetDirectoryName(RcFile.FilePath);
      var headerPath = Path.Combine(rcFileDirectory, headerNames.FirstOrDefault(
                                                                headerName => headerName.Contains("resource.h") 
                                                                              && File.Exists(Path.Combine(rcFileDirectory, headerName))
                                                                ) ?? headerNames.FirstOrDefault(
                                                                headerName => File.Exists(Path.Combine(rcFileDirectory, headerName))
                                                                ) ?? headerNames.First());
      if (!File.Exists(headerPath))
        File.Create(headerPath);
      DefaultHeaderFile = headerPath;

      mIDGenerator = new IDGenerator();
      mIDGenerator.RemoveExistingFromHeader(mHeaderContent, headerPath);

      mWriteRCPath = Path.GetTempFileName();
      mWriteHeaderPath = Path.GetTempFileName();

      if (!File.Exists(mWriteRCPath))
        File.Create(mWriteRCPath);

      if (!File.Exists(mWriteHeaderPath))
        File.Create(mWriteHeaderPath);

      mHeaderWriter = new HeaderWriter(headerPath);
      //RCFileEditor.EditValue("test_name", "edited_test_value", mRCFileContent, mHeaderContent.NameToID);

      mRCFileWriter = new RCFileWriter(mRCFileContent, rcFilePath, mShowGhostFile);
    }
    #endregion

    #region Public methods

    public void AddResource(string aValue, string aName, HeaderId aID)
    {
      var item = new TestItem()
      {
        Value = aValue,
        Name = aName,
        ID = aID
      };

      var foundIndex = HeaderAdder.AddItem(item, mHeaderContent, DefaultHeaderFile);

      // Needed for current HeaderWriter implementation, unfortunately :(
      HeaderWriter.FoundIndex = foundIndex;
      HeaderWriter.TestItem = item;

      if (foundIndex != Constants.kDuplicateID)
        RCFileAdder.AddItem(item, mRCFileContent, mHeaderContent);
    }

    public bool IdExists(HeaderId aID) => 
      mHeaderContent.GetPosition(aID, DefaultHeaderFile) == Constants.kDuplicateID;

    public bool ResourceNameExists(string aName) => mHeaderContent.NameToID.ContainsKey(aName);

    public void UpdateRCFile(IServiceProvider aServiceProvider)
    {
      mRCFileWriter.Write(mWriteRCPath, mCodePage);

      try
      {
        // Replace RC file from solution with the temp RC file created for editing
        using (var guard = new SilentFileChangerGuard(aServiceProvider, RcFile.FilePath, true))
          File.Copy(mWriteRCPath, RcFile.FilePath, true);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Unable to save RC file {0}. Reason: {1}", RcFile, ex.Message));
      }
    }

    public void UpdateHeaderFile(IServiceProvider aServiceProvider)
    {
      mHeaderWriter.Write(mWriteHeaderPath, mCodePage, mHeaderContent.SortedHeaderResults[DefaultHeaderFile].Count);

      try
      {
        // Replace header file from solution with the temp header file created for editing
        using (var guard = new SilentFileChangerGuard(aServiceProvider, DefaultHeaderFile, true))
          File.Copy(mWriteHeaderPath, DefaultHeaderFile, true);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Unable to save header file {0}. Reason: {1}", DefaultHeaderFile, ex.Message));
      }
    }

    public RCFileItem GetStringResourceByName(string aStringResourceName)
    {
      if (!HeaderContent.NameToID.ContainsKey(aStringResourceName)) return null;

      var currentResourceID = new HeaderId(HeaderContent.NameToID[aStringResourceName]);

      if (IDValidator.IsRecurrentCase(currentResourceID, HeaderContent))
        IDNormalizer.NormalizeRecurrenceForID(currentResourceID, HeaderContent);
      if (IDValidator.IsHexaRepresentation(currentResourceID))
        IDNormalizer.NormalizeHexaID(currentResourceID);

      if (!IDValidator.IsValid(currentResourceID, HeaderContent))
        return null;

      return mRCFileContent.GetStringLineForName(aStringResourceName, currentResourceID);
    }

    #endregion
  }
}

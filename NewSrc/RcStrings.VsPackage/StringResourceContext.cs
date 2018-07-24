using Microsoft.VisualStudio;
using StringEnhancer;
using System;
using System.Collections.Generic;
using System.IO;
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

    private string kDefaultResourceHeaderFileName = "resource.h";

    #endregion

    #region Properties

    public RcFile RcFile { get; private set; }
    public int GetId => mIDGenerator.Generate();
    public Dictionary<string, string> NameToID { get; }
    private string DefaultHeaderFile =>
      Path.Combine(Path.GetDirectoryName(RcFile.FilePath), kDefaultResourceHeaderFileName);
    #endregion

    #region Ctor
    /// <summary>
    /// Automatically loads the data from the RC file
    /// </summary>
    /// <param name="aRcFile"></param>
    public StringResourceContext(RcFile aRcFile)
    {
      RcFile = aRcFile;

      var rcFilePath = RcFile.FilePath;
      mCodePage = CodePageExtractor.GetCodePage(rcFilePath);

      mHeaderContentBuilder = new HeaderContentBuilder(rcFilePath, mCodePage);
      mHeaderContentBuilder.Build();
      mHeaderContent = mHeaderContentBuilder.GetResult();

      NameToID = mHeaderContent.NameToID;

      mRCFileContentBuilder = new RCFileContentBuilder(rcFilePath, mCodePage, mHeaderContent.NameToID);
      mRCFileContentBuilder.Build();
      mRCFileContent = mRCFileContentBuilder.GetResult();

      var headerPath = DefaultHeaderFile;

      mIDGenerator = new IDGenerator();
      mIDGenerator.RemoveExistingFromHeader(mHeaderContent, headerPath);

      mWriteRCPath = Path.GetTempFileName();
      mWriteHeaderPath = Path.GetTempFileName();

      if (!File.Exists(mWriteRCPath))
        File.Create(mWriteRCPath);

      if (!File.Exists(mWriteHeaderPath))
        File.Create(mWriteHeaderPath);

      mHeaderWriter = new HeaderWriter(mHeaderContent, headerPath);
      //RCFileEditor.EditValue("test_name", "edited_test_value", mRCFileContent, mHeaderContent.NameToID);

      mRCFileWriter = new RCFileWriter(mRCFileContent, rcFilePath);
    }
    #endregion

    #region Public methods

    public void AddResource(string aValue, string aName, string aID)
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
        RCFileAdder.AddItem(item, mRCFileContent, mHeaderContent.NameToID);
    }

    public bool IdExists(string aID) => 
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
      mHeaderWriter.Write(mWriteHeaderPath, mCodePage);

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
      if (!NameToID.ContainsKey(aStringResourceName)) return null;

      return mRCFileContent.GetStringLineForName(aStringResourceName, NameToID[aStringResourceName]);
    }

    #endregion
  }
}

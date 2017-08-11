using System;
using System.Collections.Generic;
using System.IO;
using Caphyon.RcStrings.StringEnhancer;
using System.Windows.Forms;

namespace Caphyon.RcStrings.VsPackage
{
  public class StringResourceContext
  {
    #region Members

    private const string kDefaultResourceHeaderFileName = "resource.h";

    private OperationsStringTable mOperationsStringTable;
    private EmptyRangeManager mEmptyRangeManager = new EmptyRangeManager();

    private RCFileContent mRcFileContent = new RCFileContent();
    private HeaderFilesContent mHeaderContent = new HeaderFilesContent();

    private RCFileParser mRcFileParser = new RCFileParser();
    private HeadersParser mHeaderParser;

    private RCFileWriter mRcFileWriter = new RCFileWriter();
    private HeaderFileWriter mHeaderFileWriter = new HeaderFileWriter();

    private IdGenerator mIdGenerator;

    private string mTempRcFile;
    private string mTempHeaderFile;

    #endregion

    #region Properties

    public RcFile RcFile { get; private set; }
    public int GetId => mIdGenerator.Generate();
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
      mHeaderParser = new HeadersParser(mHeaderContent);
      LoadStringResources(aRcFile);
      mIdGenerator = new IdGenerator(mEmptyRangeManager.GetEmptyRanges, mRcFileContent.MaximumId);
    }
    #endregion

    #region Public methods

    public void LoadStringResources(RcFile aRcFile)
    {
      RcFile = aRcFile;
      mTempRcFile = Path.GetTempFileName();
      mTempHeaderFile = Path.GetTempFileName();

      if(!File.Exists(mTempHeaderFile))
        File.Create(mTempHeaderFile);

      if (!File.Exists(mTempRcFile))
        File.Create(mTempRcFile);

      List<string> headersRelativePath = mRcFileParser.ExtractHeaders(RcFile.FilePath);
      List<string> headerFiles = new List<string>();

      foreach (string hrp in headersRelativePath)
      {
        // Handle default header file
        if (hrp.ToLower() == kDefaultResourceHeaderFileName)
        {
          headerFiles.Add(DefaultHeaderFile);
          continue;
        }

        foreach (string dir in RcFile.Project.AditionalIncludeDirectories)
        {
          string headerFullPath = Path.Combine(dir, hrp);
          string headerAbsolutPath = Path.GetFullPath((new Uri(headerFullPath)).LocalPath);
          if (File.Exists(headerAbsolutPath))
          {
            headerFiles.Add(headerAbsolutPath);
            break;
          }
        }
      }
      mRcFileContent.Headers.AddRange(headerFiles);

      mRcFileParser.ReadData(mRcFileContent, aRcFile.FilePath, mTempRcFile);
      mHeaderParser.ReadData(mRcFileContent);
      mEmptyRangeManager.FindEmptyRanges(mHeaderContent);

      mOperationsStringTable = new OperationsStringTable(mRcFileContent, mEmptyRangeManager);
    }

    public void AddResource(string aValue, string aName, int aId) =>
      mOperationsStringTable.AddStringResource(aValue, aName, aId);

    public bool IdExists(int aId) => mRcFileContent.ExistsId(aId);

    public bool ResourceNameExists(string name) => mRcFileContent.ResourceExists(name);

    public void UpdateResourceFiles(IServiceProvider aServiceProvider)
    {
      mRcFileWriter.WriteData(mRcFileContent, mTempRcFile);
      mHeaderFileWriter.WriteFile(mRcFileContent, DefaultHeaderFile, mTempHeaderFile);

      try
      {
        // Replace RC file from solution with the temp RC file created for editing
        using ( var guard = new SilentFileChangerGuard(aServiceProvider, RcFile.FilePath, true) )
          File.Copy(mTempRcFile, RcFile.FilePath, true);
      }
      catch(Exception ex)
      {
        throw new Exception(string.Format("Unable to save RC file {0}. Reason: {1}", RcFile, ex.Message));
      }

      try
      {
        // Replace header file from solution with the temp header file created for editing
        using (var guard = new SilentFileChangerGuard(aServiceProvider, DefaultHeaderFile, true))
          File.Copy(mTempHeaderFile, DefaultHeaderFile, true);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Unable to save header file {0}. Reason: {1}", DefaultHeaderFile, ex.Message));
      }
    }

    public StringLine GetStringResourceByName(string aStringResourceName) =>
      mRcFileContent.GetStringLine(aStringResourceName);

    #endregion
  }
}

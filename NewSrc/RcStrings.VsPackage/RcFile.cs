namespace Caphyon.RcStrings.VsPackage
{
  public class RcFile
  {
    #region Properties

    public string FilePath { get; set; }
    public string FileName => System.IO.Path.GetFileName(FilePath);
    public VCppProject Project { get; set; }
    public string DisplayName => $"{Project.ProjectName}: {FileName}";
    public bool IsSelectable { get; set; }

    #endregion

  }
}

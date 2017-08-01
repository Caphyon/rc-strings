using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.VsPackage
{
  public class RcFile
  {
    public string FilePath { get; private set; }
    public string FileName
    {
      get => System.IO.Path.GetFileName(FilePath);
    }
    public VCppProject Project { get; set; }
    public string DisplayName
    {
      get => string.Format("{0}: {1}", Project.ProjectName, FileName);
    }
    public RcFile(string aFilePath) => FilePath = aFilePath;

  }
}

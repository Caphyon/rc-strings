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
    #region Propertries

    public string FilePath { get; private set; }
    public string FileName => System.IO.Path.GetFileName(FilePath);
    public VCppProject Project { get; set; }
    public string DisplayName => string.Format("{0}: {1}", Project.ProjectName, FileName);

    #endregion

    #region Ctor

    public RcFile(string aFilePath) => FilePath = aFilePath;

    #endregion
  }
}

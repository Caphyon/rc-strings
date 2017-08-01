using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace Caphyon.RcStrings.VsPackage
{
  public class VCppProject
  {
    public string ProjectName { get; private set; }
    public List<string> AditionalIncludeDirectories { get; }

    public VCppProject(Project aProject)
    {
      ProjectName = aProject.Name;
      AditionalIncludeDirectories = new List<string>();
    }

    public VCppProject() {}
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.VsPackage
{
  [Serializable]
  public class UserSettings
  {
    private List<RcFileInfo> mSolutionsSelectedRc = new List<RcFileInfo>();

    public List<RcFileInfo> SolutionsSelectedRc
    {
      get => mSolutionsSelectedRc;
      set => mSolutionsSelectedRc = value;
    }
  }

  [Serializable]
  public class RcFileInfo
  {
    public string SolutionName { get; set; }

    public string ProjectName { get; set; }

    public string SelectedRc { get; set; }

    public string ReplaceWith { get; set; }

    public bool IsReplacingWith { get; set; }
  }

}

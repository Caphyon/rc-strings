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
    #region Members

    private List<RcFileInfo> mSolutionsSelectedRc = new List<RcFileInfo>();

    #endregion

    #region Properties

    public List<RcFileInfo> SolutionsSelectedRc
    {
      get => mSolutionsSelectedRc;
      set => mSolutionsSelectedRc = value;
    }

    #endregion
  }

  [Serializable]
  public class RcFileInfo
  {
    #region Properties

    public string SolutionName { get; set; }

    public string ProjectName { get; set; }

    public string SelectedRc { get; set; }

    public string ReplaceWith { get; set; }

    public bool IsReplacingWith { get; set; }

    #endregion
  }

}

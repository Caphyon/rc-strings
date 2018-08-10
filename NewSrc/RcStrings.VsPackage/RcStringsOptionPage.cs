using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace Caphyon.RcStrings.VsPackage
{
  public class RcStringsOptionPage : DialogPage
  {
    [Category("String resource")]
    [DisplayName("Randomize ID")]
    [Description("If set to True the ID will be randomly generated. Otherwise the ID will be the first available.")]
    public bool RandomId { get; set; }

    [Category("String resource")]
    [DisplayName("Unique ID per project")]
    [Description("If set to True the ID will be uniquely generated for all the RC files in the same project. Otherwise the ID will be uniquely generated just for the selected RC file.")]
    public bool IDUniquenessPerProject { get; set; } = true;

    [Category("String resource")]
    [DisplayName("Show ghost entries files")]
    [Description("If set to True the found strings from a RC file which don't have a corresponding ID in a header file will be showed for every \"Add\" command. Otherwise they will not be showed.")]
    public bool ShowGhostFile { get; set; } = false;
  }
}

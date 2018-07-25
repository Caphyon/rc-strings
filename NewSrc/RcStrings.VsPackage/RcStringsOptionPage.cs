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
    [DisplayName("ID uniqueness per project")]
    [Description("If set to True the ID will be generated uniquely for all RCs in the project. Otherwise the ID will be generated uniquely for selected RC.")]
    public bool IDUniquenessPerProject { get; set; } = true;
  }
}

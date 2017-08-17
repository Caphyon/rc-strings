using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace Caphyon.RcStrings.VsPackage
{
  public class RcStringsOptionPage : DialogPage
  {
    [Category("String resource")]
    [DisplayName("Randomize Id")]
    [Description("If set to true the id will be randomly generated. Otherwise the id will be the first available.")]
    public bool RandomId { get; set; }
  
  }
}

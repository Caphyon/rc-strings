using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.VsPackage
{
  public class RandomIdOption : DialogPage
  {
    [Category("String resource")]
    [DisplayName("Randomize Id")]
    [Description("If set to true the id will be randomly generated. Otherwise the id will be the first available.")]
    public bool RandomId { get; set; }
  
  }
}

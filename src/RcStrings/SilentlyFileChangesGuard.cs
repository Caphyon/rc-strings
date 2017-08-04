using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.VsPackage
{
  public class SilentlyFileChangesGuard : SilentlyFileChanges, IDisposable
  {
    public SilentlyFileChangesGuard(IServiceProvider aSite, string aDocument, bool aReloadDocument)
      : base(aSite, aDocument, aReloadDocument) => Suspend();

    public void Dispose() => Resume();
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.VsPackage
{
  public class SilentFileChangerGuard : SilentFileChanger, IDisposable
  {
    #region Public methods

    public SilentFileChangerGuard(IServiceProvider aSite, string aDocument, bool aReloadDocument)
      : base(aSite, aDocument, aReloadDocument) => Suspend();

    public void Dispose() => Resume();

    #endregion
  }
}

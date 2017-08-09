using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class EmptyRange
  {
    #region Properties

    public int StartPosition { get; private set; }
    public int StopPosition { get; private set; }

    #endregion

    #region Ctor

    public EmptyRange(int aStart, int aStop)
    {
      StartPosition = aStart;
      StopPosition = aStop;
    }

    #endregion
  }
}

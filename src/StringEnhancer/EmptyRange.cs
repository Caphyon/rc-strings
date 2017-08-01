using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class EmptyRange
  {
    public int StartPosition { get; private set; }
    public int StopPosition { get; private set; }

    public EmptyRange(int aStart, int aStop)
    {
      StartPosition = aStart;
      StopPosition = aStop;
    }
  }
}

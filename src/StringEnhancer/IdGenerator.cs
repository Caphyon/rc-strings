using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class IdGenerator : IGenerator<int>
  {
    #region Members

    private SortedSet<EmptyRange> mEmptyRanges;
    public static readonly int kMaximumId = 65535;

    #endregion

    #region Properties

    public int ExistingMaximumId { get; private set; }
    public Random RandomNumber { get; private set; }

    #endregion

    #region Public methods

    public IdGenerator(SortedSet<EmptyRange> aEmptyRanges, int aId)
    {
      RandomNumber = new Random(DateTime.Now.Millisecond);
      mEmptyRanges = aEmptyRanges;
      ExistingMaximumId = aId;
    }

    public int Generate()
    {
      if ( mEmptyRanges.Count != 0 )
      {
        var emptyRange = mEmptyRanges.ElementAt(RandomNumber.Next(mEmptyRanges.Count));
        return RandomNumber.Next(emptyRange.StartPosition, emptyRange.StopPosition + 1);
      }
      else
        return RandomNumber.Next(ExistingMaximumId, kMaximumId);
    }

    #endregion
  }
}

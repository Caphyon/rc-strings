using System;
using System.Collections.Generic;
using System.Linq;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class IdGenerator : IGenerator<int>
  {
    #region Members

    private SortedSet<EmptyRange> mEmptyRanges;
    public static readonly int kMaximumId = 65535;
    #endregion

    #region Properties

    private int ExistingMaximumId { get; set; }
    private Random RandomNumber { get; set; }
    public static bool RandomId { get; set; }
    #endregion

    #region Ctor

    public IdGenerator(SortedSet<EmptyRange> aEmptyRanges, int aId)
    {
      RandomNumber = new Random(DateTime.Now.Millisecond);
      mEmptyRanges = aEmptyRanges;
      ExistingMaximumId = aId;
    }
    #endregion

    #region Public methods

    public int Generate()
    {
      return RandomId ? Random() : Next();
    }
    #endregion

    #region Public Methods

    private int Random()
    {
      if (mEmptyRanges.Count != 0)
      {
        var emptyRange = mEmptyRanges.ElementAt(RandomNumber.Next(mEmptyRanges.Count));
        return RandomNumber.Next(emptyRange.StartPosition, emptyRange.StopPosition + 1);
      }
      else
        return RandomNumber.Next(ExistingMaximumId, kMaximumId);
    }

    private int Next()
    {
      return mEmptyRanges.Count != 0 ?
        mEmptyRanges.First().StartPosition : ExistingMaximumId + 1;
    }
    #endregion
  }
}

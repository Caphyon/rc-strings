using System;
using System.Collections.Generic;

namespace StringEnhancer
{
  public class DuplicateKeyComparer<TKey>
    : IComparer<TKey> where TKey : IComparable
  {
    public int Compare(TKey aFirst, TKey aSecond)
    {
      int result = aFirst.CompareTo(aSecond);

      if (result == 0)
      {
        return 1;
      }

      return result;
    }
  }
}
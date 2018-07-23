using System;
using System.Collections.Generic;

namespace StringEnhancer
{
  public class HeaderResultComparerByID : IComparer<HeaderItem>
  {
    public int Compare(HeaderItem aFirst, HeaderItem aSecond)
    {
      return Convert.ToInt32(aFirst.ID).CompareTo(Convert.ToInt32(aSecond.ID));
    }
  }
}
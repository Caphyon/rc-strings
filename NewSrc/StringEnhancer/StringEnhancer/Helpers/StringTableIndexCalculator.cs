using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringEnhancer
{
  public static class StringTableIndexCalculator
  {
    public static int CalculateIndex(string aID)
    {
      return Convert.ToInt32(aID) / Constants.kStringTableCapacity;
    }
  }
}

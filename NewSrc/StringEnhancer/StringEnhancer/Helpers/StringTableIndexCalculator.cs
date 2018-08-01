using System;

namespace StringEnhancer
{
  public static class StringTableIndexCalculator
  {
    public static int CalculateIndex(HeaderId aID)
    {
      return Convert.ToInt32(IDNormalizer.CopyNormalizeHexaID(aID).Value) / Constants.kStringTableCapacity;
    }
  }
}

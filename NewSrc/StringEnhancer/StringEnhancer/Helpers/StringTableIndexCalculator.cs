using System;

namespace StringEnhancer
{
  public static class StringTableIndexCalculator
  {
    public static int CalculateIndex(HeaderId aID)
    {
      var copiedId = new HeaderId(aID);

      if (copiedId.IsHexa)
        IDNormalizer.NormalizeHexaID(copiedId);

      return Convert.ToInt32(copiedId.Value) / Constants.kStringTableCapacity;
    }
  }
}

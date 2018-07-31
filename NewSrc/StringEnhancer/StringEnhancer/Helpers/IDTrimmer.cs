using System;

namespace StringEnhancer
{
  public static class IDTrimmer
  {
    private static readonly char[] kTrimConstants = { '\t', '/' };

    public static void TrimEnd(HeaderId aID) =>
      TrimEnd(aID.Value);
    public static void TrimEnd(string aString) =>
      aString = aString.TrimEnd(kTrimConstants);

    public static void TrimStart(HeaderId aID) =>
      TrimStart(aID.Value);
    public static void TrimStart(string aString) =>
      aString = aString.TrimStart(kTrimConstants);

    public static void Trim(HeaderId aID) =>
      Trim(aID.Value);
    public static void Trim(string aString)
    {
      TrimStart(aString);
      TrimEnd(aString);
    }
  }
}
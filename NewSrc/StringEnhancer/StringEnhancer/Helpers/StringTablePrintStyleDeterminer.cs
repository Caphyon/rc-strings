namespace StringEnhancer
{
  public static class StringTablePrintStyleDeterminer
  {
    public static StringTablePrintStyle DeterminePrintStyle(string aName, string aValue)
    {
      if (aName.Length >= 24 && aValue.Length + aName.Length >= 74)
      {
        return StringTablePrintStyle.NewLine;
      }
      else
      {
        return StringTablePrintStyle.Joined;
      }
    }
  }
}

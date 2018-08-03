using System;

namespace StringEnhancer.Serialization
{
  public static class Extensions
  {
    public static string Serialize(this HeaderItem aResult)
    {
      return $"ID: {aResult.ID.Value}\r\nName: {aResult.Name}";
    }

    public static string Serialize(this RCFileItem aResult)
    {
      string toPrint;

      if (aResult.PrintStyle == StringTablePrintStyle.Joined)
        toPrint = $"    {aResult.Name.PadRight(23)} {aResult.Value}";
      else if (aResult.PrintStyle == StringTablePrintStyle.NewLine)
        toPrint = $"    {aResult.Name} \r\n{aResult.Value.PadLeft(28 + aResult.Value.Length)}";
      else
        toPrint = $"Name: {aResult.Name}\r\nValue: {aResult.Value}\r\n";

      return toPrint;
    }

    public static string Serialize(this LineParserResult aResult)
    {
      return aResult.Name;
    }

    public static string Serialize(this HeaderId aResult)
    {
      return aResult.Value;
    }

    public static string SerializeForHeader(this TestItem aResult)
    {
      return $@"#define {aResult.Name.PadRight(Math.Max(aResult.Name.Length, 31))} {aResult.ID.Serialize()}";
    }
  }
}

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace StringEnhancer
{
  public class HeaderParser : AbstractParser<HeaderItem>
  {
    public HeaderParser(string aPath, Encoding aCodePage) : base(aPath, aCodePage) { }
    
    int mIgnoreValue = 0; // Skip #ifdef #ifndef content, positive value means ignore

    protected override void DoParse()
    {
      string line;
      mResult = null; // Reset parse result

      while ((line = mFileStream.ReadLine()) != null)
      {
        ParseLine(line);
        if (HasNextCondition()) break;
      }
    }

    private void ParseLine(string aLine)
    {
      //if (aLine.Length == 0 || aLine[0] != '#') return;

      aLine.TrimStart();
      if (aLine.StartsWith("#if") && Regex.IsMatch(aLine, "INVOKED"))
      {
        ++mIgnoreValue;
      }
      else if (aLine.StartsWith("#end") && mIgnoreValue > 0)
      {
        --mIgnoreValue;
      }

      if (mIgnoreValue > 0) return;

      string[] words = aLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      if (words.Length <= 2 || words[0] != "#define")
        return;

      mResult = new HeaderItem
      {
        Name = words[1],
        ID = words[2]
      };
    }

    protected override HeaderItem GetResult()
    {
      return mResult;
    }

    protected override bool HasNextCondition()
    {
      return mResult != null;
    }
  }
}
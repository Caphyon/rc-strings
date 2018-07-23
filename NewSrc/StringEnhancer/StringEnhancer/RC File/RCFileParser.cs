using System;
using System.Text;

namespace StringEnhancer
{
  public class RCFileParser : AbstractParser<RCFileItem>
  {
    public RCFileParser(string aPath, Encoding aCodePage) : base(aPath, aCodePage) { }

    bool mIsInStringTable = false;
    string mLine;

    protected override void DoParse()
    {
      mResult = null;

      while ((mLine = mFileStream.ReadLine()) != null)
      {
        ParseLine(ref mLine, ref mIsInStringTable);
        if (HasNextCondition()) break;
      }
    }

    private void ParseLine(ref string aLine, ref bool aIsInStringTable)
    {
      if (aLine == "STRINGTABLE")
      {
        aIsInStringTable = true;
        return;
      }

      if (aIsInStringTable)
      {
        mResult = new RCFileItem();
        string[] words = aLine.Trim().Split();

        mResult.Name = words[0];

        if (aLine == "BEGIN") return;
        if (aLine == "END") aIsInStringTable = false;
        else // Build Value
        {
          if (words.Length == 1)
          {
            mResult.Value = mFileStream.ReadLine()?.TrimStart();
            mResult.PrintStyle = StringTablePrintStyle.NewLine;
          }
          else
          {
            mResult.Value = String.Join(" ", words, 1, words.Length - 1).TrimStart();
            mResult.PrintStyle = StringTablePrintStyle.Joined;
          }
        }
      }
    }

    protected override RCFileItem GetResult()
    {
      return mResult;
    }

    protected override bool HasNextCondition()
    {
      return mResult != null;
    }
  }
}
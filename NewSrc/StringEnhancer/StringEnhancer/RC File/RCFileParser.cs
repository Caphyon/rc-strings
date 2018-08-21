using System;
using System.Linq;
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
        aLine = aLine.Trim();

        if (aLine.Length == 0) return;

        var words = aLine.Split(Constants.kSplitTokens);
        if (words.Length == 0) return;
        mResult.Name = words[0];

        if (aLine == "BEGIN") return;
        if (aLine == "END") aIsInStringTable = false;

        var stringBuilder = new StringBuilder();
        var word = string.Join(" ", words, 1, words.Length - 1).Trim(Constants.kSplitTokens);

        if (words.Length == 1)
        {
          mResult.PrintStyle = StringTablePrintStyle.NewLine;
          stringBuilder.Append(mFileStream.ReadLine()?.TrimStart(Constants.kSplitTokens) ?? "");
        }
        else if (!word.EndsWith("\\"))
        {
          mResult.PrintStyle = StringTablePrintStyle.Joined;
          stringBuilder.Append(word);
        }
        else
        {
          mResult.PrintStyle = StringTablePrintStyle.Joined;
          stringBuilder.Append(word);

          while (word.EndsWith("\\"))
          {
            stringBuilder.Length--;
            word = mFileStream.ReadLine()?.TrimStart(Constants.kSplitTokens) ?? "\"";
            stringBuilder.Append(word);
          }
        }

        mResult.Value = stringBuilder.ToString();
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
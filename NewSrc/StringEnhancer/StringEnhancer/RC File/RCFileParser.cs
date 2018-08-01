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

        mResult.Name = aLine.Split(Constants.kSplitTokens)[0];

        if (aLine == "BEGIN") return;
        if (aLine == "END") aIsInStringTable = false;

        var firstQuote = aLine.IndexOf('\"');
        var lastQuote = aLine.LastIndexOf('\"');

        if (firstQuote != -1 && lastQuote != -1)
        {
          mResult.Value = aLine.Substring(firstQuote, lastQuote - firstQuote + 1);
          mResult.PrintStyle = StringTablePrintStyle.Joined;
        }
        else
        {
          mResult.Value = mFileStream.ReadLine()?.TrimStart(Constants.kSplitTokens);
          mResult.PrintStyle = StringTablePrintStyle.NewLine;
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
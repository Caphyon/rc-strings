using System.Text;

namespace StringEnhancer
{
  public class LineParser : AbstractParser<LineParserResult>
  {
    public LineParser(string aPath) : base(aPath, Encoding.GetEncoding(0)) { }
    public LineParser(string aPath, Encoding aCodePage) : base(aPath, aCodePage) { }

    protected override void DoParse()
    {
      mResult = null;
      var line = mFileStream.ReadLine();
      if (line != null)
      {
        mResult = new LineParserResult { Name = line };
      }
    }

    protected override LineParserResult GetResult()
    {
      return mResult;
    }

    protected override bool HasNextCondition()
    {
      return mResult != null;
    }
  }
}
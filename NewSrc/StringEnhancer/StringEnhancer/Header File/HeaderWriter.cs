using StringEnhancer.Serialization;
using System;
using System.IO;
using System.Text;

namespace StringEnhancer
{
  public class HeaderWriter
  {
    private HeaderContent mHeaderContent;
    private string mHeaderPath;

    public static TestItem TestItem { get; set; }
    public static int FoundIndex { get; set; }

    public HeaderWriter(HeaderContent aHeaderContent, string aHeaderPath)
    {
      mHeaderContent = aHeaderContent;
      mHeaderPath = aHeaderPath;
    }

    public void Write(string aWritePath, Encoding aCodePage)
    {
      using (var writeFile = new StreamWriter(aWritePath, false, aCodePage))
      {
        using (var lineParser = new LineParser(mHeaderPath))
        {
          var sortedHeaderResults = mHeaderContent.SortedHeaderResults;
          var nameToID = mHeaderContent.NameToID;

          //////////////////////////////////////////////
          // Resolve ID (conversions, etc...)
          //TestItem.ID = IDNormalizer.NormalizeID(TestItem.ID, nameToID);
          //TestItem.ID = IDNormalizer.NormalizeReccurenceForID(obj.ID, mHeaderContent.NameToID);

          int line_count = -1;
          LineParserResult line = null;

          while (lineParser.HasNext())
          {
            line = lineParser.GetNext();

            if (line.Name.Split()[0] == "#define") ++line_count;

            if (line_count == FoundIndex) break;
            writeFile.WriteLine(line.Serialize());
          }

          writeFile.WriteLine($@"#define {TestItem.Name.PadRight(Math.Max(TestItem.Name.Length, 31))} {TestItem.ID}");

          if (!lineParser.HasNext()) return;
          writeFile.WriteLine(line.Serialize());

          while (lineParser.HasNext())
          {
            writeFile.WriteLine(lineParser.GetNext().Serialize());
          }
        }
      }
    }
  }
}
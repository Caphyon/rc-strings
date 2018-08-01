using StringEnhancer.Serialization;
using System;
using System.IO;
using System.Text;

namespace StringEnhancer
{
  public class HeaderWriter
  {
    private readonly string mHeaderPath;

    public static TestItem TestItem { get; set; }
    public static int FoundIndex { get; set; }

    public HeaderWriter(string aHeaderPath)
    {
      mHeaderPath = aHeaderPath;
    }

    public void Write(string aWritePath, Encoding aCodePage)
    {
      using (var writeFile = new StreamWriter(aWritePath, false, aCodePage))
      {
        using (var lineParser = new LineParser(mHeaderPath))
        {
          var lineCount = -1;
          var ignoreValue = 0;

          while (lineParser.HasNext())
          {
            var line = lineParser.GetNext();
            var words = line.Name.Split(Constants.kSplitTokens, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0) continue;

            writeFile.WriteLine(line.Serialize());

            if (words[0].StartsWith("#if"))
              ++ignoreValue;
            else if (words[0].StartsWith("#end") && ignoreValue > 0)
              --ignoreValue;

            if (ignoreValue > 0) continue;

            if (words[0] == "#define") ++lineCount;

            if (lineCount == FoundIndex - 1) break;
          }
          
          writeFile.WriteLine(TestItem.SerializeForHeader());

          while (lineParser.HasNext())
          {
            writeFile.WriteLine(lineParser.GetNext().Serialize());
          }
        }
      }
    }
  }
}
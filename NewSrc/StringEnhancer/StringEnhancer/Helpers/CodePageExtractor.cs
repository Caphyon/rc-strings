using System;
using System.Text;
using System.Text.RegularExpressions;

namespace StringEnhancer
{
  public class CodePageExtractor
  {
    public static Encoding GetCodePage(string aPath)
    {
      using (var lineParser = new LineParser(aPath))
      {
        while (lineParser.HasNext())
        {
          var line = lineParser.GetNext();

          string[] words = line?.Name.Split(Constants.kSplitTokens, StringSplitOptions.RemoveEmptyEntries);
          if (words.Length == 0) continue;

          if (words[0] == "#pragma")
          {
            return Encoding.GetEncoding(Convert.ToInt32(Regex.Match(words[1], @"\d+").Value));
          }
        }

        return Encoding.GetEncoding(0);
      }
    }
  }
}

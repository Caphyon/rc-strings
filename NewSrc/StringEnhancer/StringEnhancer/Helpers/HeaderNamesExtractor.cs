using System.Collections.Generic;
using System.Text;

namespace StringEnhancer
{
  public static class HeaderNamesExtractor
  {
    public static IEnumerable<string> ExtractHeaderNames(string aPath, Encoding aCodePage)
    {
      var headerNames = new List<string>();
      using (var lineParser = new LineParser(aPath))
      {

        while (lineParser.HasNext())
        {
          var line = lineParser.GetNext();

          string[] words = line.Name.Split();

          if (words[0] == "#include")
          {
            words[1] = words[1].Trim('\"');
            headerNames.Add(words[1]);
          }
          else if (words[0] == "BEGIN")
          {
            if (!headerNames.Contains("resource.h"))
              headerNames.Clear();
            return headerNames;
          }
        }

        return headerNames;
      }
    }
  }
}

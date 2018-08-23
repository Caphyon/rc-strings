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
    public static string SearchedName { get; set; }

    public HeaderWriter(string aHeaderPath)
    {
      mHeaderPath = aHeaderPath;
    }

    public void WriteForAdd(string aWritePath, Encoding aCodePage, int aNumberOfElementsInHeader)
    {
      using (var writeFile = new StreamWriter(aWritePath, false, aCodePage))
      {
        using (var lineParser = new LineParser(mHeaderPath, aCodePage))
        {
          var lineCount = -1;
          var ignoreValue = 0;
          LineParserResult line = null;

          while (lineParser.HasNext())
          {
            line = lineParser.GetNext();

            var words = line.Name.Split(Constants.kSplitTokens, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
              writeFile.WriteLine(line.Serialize());
              continue;
            }

            if (words[0].StartsWith("#if"))
              ++ignoreValue;
            else if (words[0].StartsWith("#endif") && ignoreValue > 0)
              --ignoreValue;
            
            if (ignoreValue > 0 || words[0] != "#define")
            {
              writeFile.WriteLine(line.Serialize());
              continue;
            }
            
            ++lineCount;

            if (lineCount == FoundIndex)
              break;

            writeFile.WriteLine(line.Serialize());

            if (lineCount + 2 == aNumberOfElementsInHeader)
              break;
          }

          writeFile.WriteLine(TestItem.SerializeForHeader());

          // Check for no extra writing
          if (lineCount == FoundIndex)
            writeFile.WriteLine(line.Serialize());

          while (lineParser.HasNext())
          {
            writeFile.WriteLine(lineParser.GetNext().Serialize());
          }
        }
      }
    }

    public void WriteForEdit(string aWritePath, Encoding aCodePage)
    {
      using (var writeFile = new StreamWriter(aWritePath, false, aCodePage))
      {
        using (var lineParser = new LineParser(mHeaderPath, aCodePage))
        {
          while (lineParser.HasNext())
          {
            var line = lineParser.GetNext();

            var words = line.Name.Split(Constants.kSplitTokens, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
              writeFile.WriteLine(line.Serialize());
              continue;
            }

            if (words[0].Equals("#define") && words[1].Equals(SearchedName) && words[2].Equals(TestItem.ID.Value))
            {
              writeFile.WriteLine(TestItem.SerializeForHeader());
            }
            else
            {
              writeFile.WriteLine(line.Serialize());
            }
          }
        }
      }
    }
  }
}
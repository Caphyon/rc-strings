﻿using StringEnhancer.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringEnhancer
{
  public class RCFileWriter
  {
    private RCFileContent mRCFileContent;
    private readonly string mRCPath;

    public RCFileWriter(RCFileContent aRcFileContent, string aRCPath)
    {
      mRCFileContent = aRcFileContent;
      mRCPath = aRCPath;
    }

    public void Write(string aWritePath, Encoding aCodePage)
    {
      using (var writeFile = new StreamWriter(aWritePath, false, aCodePage))
      {
        var stringTableContent = mRCFileContent.StringTableContent;
        var stringTableIndexOrder = mRCFileContent.StringTableIndexOrder;

        using (var lineParser = new LineParser(mRCPath))
        {
          WriteUntilStringTables(writeFile, lineParser);
          WriteStringTables(writeFile, stringTableContent, stringTableIndexOrder);
          WriteUntilEnd(writeFile, lineParser);
        }
      }
    }

    private void WriteUntilEnd(StreamWriter aWriteFile, LineParser aLineParser)
    {
      while (aLineParser.HasNext())
      {
        var line = aLineParser.GetNext();
        if (line.Name.Length > 0 && line.Name[0] == '#')
        {
          aWriteFile.WriteLine(line.Serialize());
          break;
        }
      }

      while (aLineParser.HasNext())
      {
        aWriteFile.WriteLine(aLineParser.GetNext().Serialize());
      }
    }

    private void WriteStringTables(StreamWriter aWriteFile, Dictionary<int, List<RCFileItem>> aStringTableContent, List<int> aStringTableIndexOrder)
    {
      // Building unusedContentFilePath and unusedContentFile
      var lastIndexOfQuotes = mRCPath.LastIndexOf('\\');
      var unusedContentFilePath = mRCPath.Substring(0, lastIndexOfQuotes) + "\\unused_content_" + mRCPath.Substring(lastIndexOfQuotes + 1) + ".txt";
      var unusedContentFile = new StreamWriter(unusedContentFilePath, false, aWriteFile.Encoding);

      foreach (var idx in aStringTableIndexOrder)
      {
        for (int i = 0; i < aStringTableContent[idx].Count;)
        {
          if (i % Constants.kStringTableCapacity == 0)
          {
            aWriteFile.WriteLine("STRINGTABLE");
            aWriteFile.WriteLine("BEGIN");
          }
          
          var currentItem = aStringTableContent[idx][i];
          aWriteFile.WriteLine(currentItem.Serialize());

          if (currentItem.ID == Constants.kNotFoundID)
          {
            // Write in unused_content.txt file if adding
            var objPrintStyle = currentItem.PrintStyle;
            currentItem.PrintStyle = StringTablePrintStyle.Debug;
            unusedContentFile.WriteLine(currentItem.Serialize());
            currentItem.PrintStyle = objPrintStyle;
          }

          ++i;

          if (i % Constants.kStringTableCapacity == 0 || i == aStringTableContent[idx].Count)
          {
            aWriteFile.WriteLine("END\r\n");
          }
        }
      }

      unusedContentFile.Close();
    }

    private void WriteUntilStringTables(StreamWriter aWriteFile, LineParser aLineParser)
    {
      while (aLineParser.HasNext())
      {
        var line = aLineParser.GetNext();
        if (line.Name == "STRINGTABLE") break;
        aWriteFile.WriteLine(line.Serialize());
      }
    }
  }
}
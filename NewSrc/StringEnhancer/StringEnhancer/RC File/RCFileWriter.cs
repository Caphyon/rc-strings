using StringEnhancer.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StringEnhancer
{
  public class RCFileWriter
  {
    private RCFileContent mRCFileContent;
    private readonly string mRCPath;
    private readonly bool mShowGhostFile;

    public RCFileWriter(RCFileContent aRcFileContent, string aRCPath, bool aShowGhostFile)
    {
      mRCFileContent = aRcFileContent;
      mRCPath = aRCPath;
      mShowGhostFile = aShowGhostFile;
    }

    public void Write(string aWritePath, Encoding aCodePage)
    {
      using (var writeFile = new StreamWriter(aWritePath, false, aCodePage))
      {
        var stringTableContent = mRCFileContent.StringTableContent;
        var stringTableIndexOrder = mRCFileContent.StringTableIndexOrder;

        using (var lineParser = new LineParser(mRCPath, aCodePage))
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
      string unusedContentFilePath = null;
      StreamWriter unusedContentFile = null;
      var isUnusedFileEmpty = true;

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

          if (currentItem.ID.Value == Constants.kNotFoundID.Value)
          {
            if (isUnusedFileEmpty)
            {
              unusedContentFilePath = Path.GetTempFileName();
              unusedContentFile = new StreamWriter(unusedContentFilePath, false, aWriteFile.Encoding);
              isUnusedFileEmpty = false;

              unusedContentFile.WriteLine("////////////////////////////////////////////////////////////////////////////////////////////////\r\n" +
                                          "This file is generated when GHOST entries are detected in your current .rc file.\r\n" +
                                          "GHOST entry means a string resource from the .rc file that has defined ID in any header included\r\n" +
                                          "////////////////////////////////////////////////////////////////////////////////////////////////\r\n");
            }

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
      
      if (isUnusedFileEmpty || !mShowGhostFile) return;
      unusedContentFile.Close();
      var editorProcess = Process.Start("notepad.exe", unusedContentFilePath);
      editorProcess.EnableRaisingEvents = true;
      editorProcess.Exited += (sender, args) => File.Delete(unusedContentFilePath);
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
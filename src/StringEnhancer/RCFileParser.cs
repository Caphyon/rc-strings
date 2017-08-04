using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class RCFileParser : Parse
  {
    #region Public methods

    public void ReadData(RCFileContent aRcFileContent, string aPathRCFile, string aPathRCFileWiter)
    {
      int stringTableOrder = 0;
      bool stringTableFound = false;

      if (FileExists(aPathRCFile))
      {
        aRcFileContent.CodePage = ExtractCodePage(aPathRCFile);

        using (StreamReader readFile = new StreamReader(aPathRCFile, aRcFileContent.RcEncoding))
        using (StreamWriter streamWriter = new StreamWriter(aPathRCFileWiter, false, aRcFileContent.RcEncoding))
        {
          string readLine;
          while (!readFile.EndOfStream)
          {
            readLine = readFile.ReadLine();

            // STRINGTABLE found
            if (readLine.Trim() == TagConstants.kTagStringTable)
            {
              stringTableFound = true;
              // skip BEGIN TAG
              readLine = readFile.ReadLine();

              ReadStringTableContent(aRcFileContent, readFile, stringTableOrder);
              ++stringTableOrder;
            }

            //Read the informations after the last StringTable
            if (stringTableFound && readLine.Contains(TagConstants.kTagEndif))
            {
              ReadEndOfRcFile(aRcFileContent, readFile, readLine);
              break;
            }

            //Write all until the first StringTable
            if (!stringTableFound)
              streamWriter.WriteLine(readLine);
          }
        }
      }
      else
        throw new FileNotFoundException(aPathRCFile);
    }

    /// <summary>
    /// Extract header files path relative to .cxproj file
    /// </summary>
    /// <param name="pathRCFile"></param>
    public List<string> ExtractHeaders(string aPathRCFile)
    {
      List<string> headerFilesPath = new List<string>();
      using (StreamReader reader = new StreamReader(aPathRCFile))
      {
        string line = string.Empty;
        while (line != TagConstants.kTagBegin && !reader.EndOfStream)
        {
          line = reader.ReadLine();
          if (line.Contains(TagConstants.kTagInclude))
          {
            // Skip the "#include " tag and the empty space after it 
            string headerRelativePath = line.Substring(TagConstants.kTagInclude.Length + 1);
            // Remove escape sequences
            headerRelativePath = headerRelativePath.Replace("\"", string.Empty);
            headerRelativePath = headerRelativePath.Replace("/", "\\");
            headerFilesPath.Add(headerRelativePath);
          }
        }
      }
      return headerFilesPath;
    }

    #endregion

    #region Private methods

    private int ExtractCodePage(string aPathRCFile)
    {
      const string regex = @"-?\d+";
      string codePage = string.Empty;

      using (StreamReader readFile = new StreamReader(aPathRCFile))
      {
        string readLine = string.Empty;
        while (!readFile.EndOfStream && readLine != TagConstants.kTagBegin)
        {
          readLine = readFile.ReadLine();
          if (readLine.Contains(TagConstants.kTagCodePage))
          {
            codePage = Regex.Match(readLine, regex).Value;
            break;
          }
        }
      }
      return codePage != string.Empty ? (int.Parse(codePage)) : 0;
    }

    private void ReadStringTableContent(RCFileContent aRcFileContent, StreamReader aReadFile, int aStringTableOrder)
    {
      string readLine = string.Empty;

      while ((readLine = aReadFile.ReadLine()) != TagConstants.kTagEnd)
      {
        string line = readLine;
        string stringValue = string.Empty;
        List<string> lineComponentsList = ParseUtility.BuildListOfStringsFromReadLine(readLine, new char[] { ' ' });

        if (lineComponentsList.Count < 1)
          continue;
        ExtractStringValueLine(aReadFile, lineComponentsList, ref stringValue, ref line);
        BuildStringValue(ref line, ref stringValue, aReadFile);

        stringValue = new EscapeCharacters().Escape(stringValue);
        SaveData(aRcFileContent, lineComponentsList[0], stringValue, line, aStringTableOrder);
      }
    }

    private void ExtractStringValueLine(StreamReader aReadFile, List<string> aLineComponentsList,
                                          ref string aStringValue, ref string aLine)
    {
      string readLine = string.Empty;

      //IDC_RENAME_BUILD
      //                  "Rename the build"
      if (aLineComponentsList.Count == 1)
        do
        {
          readLine += aReadFile.ReadLine();
          aStringValue = readLine.Trim('"').Trim(' ');
          aLine += "\r\n" + readLine;
        } while (string.IsNullOrWhiteSpace(aStringValue));
      //IDC_RENAME_BUILD  "Rename the build"
      else
      {
        for (int index = 1; index < aLineComponentsList.Count; ++index)
          aStringValue += aLineComponentsList[index] + " ";
        aStringValue = aStringValue.Trim('"').Trim(' ');
      }
    }

    private void BuildStringValue(ref string aLine, ref string aStringValue, StreamReader aReadFile)
    {
      string readLine;
      while (aLine[aLine.Length - 1] == '\\')
      {
        readLine = aReadFile.ReadLine();
        aStringValue += readLine.Trim();
        aLine += "\r\n" + readLine;
      }
    }

    private void SaveData(RCFileContent aRcFileContent, string aName, string aValue,
      string aLine, int aStringTableOrder)
    {
      StringLine stringLine = new StringLine(aName, aValue, aLine);
      stringLine.RcOrder = aStringTableOrder;
      aRcFileContent.AddInStringLines(stringLine);
    }

    private void ReadEndOfRcFile(RCFileContent aRcFileContent, StreamReader aReadFile, string aReadLine)
    {
      aRcFileContent.EndRcFile += aReadLine + "\r\n";
      do
      {
        aReadLine = aReadFile.ReadLine();
        aRcFileContent.EndRcFile += aReadLine + "\r\n";
      } while (!aReadFile.EndOfStream);
    }

    #endregion
  }
}


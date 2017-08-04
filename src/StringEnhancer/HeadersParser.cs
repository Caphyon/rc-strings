using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class HeadersParser : Parse
  {
    #region Members

    private HeaderFilesContent mHeaderContent;

    #endregion

    #region Ctor

    public HeadersParser(HeaderFilesContent aHeaderContent) => mHeaderContent = aHeaderContent;

    #endregion

    #region Public methods


    public void ReadData(RCFileContent aRcFileContent)
    {
      var headers = aRcFileContent.Headers;
      bool wasSaveStringsWithEmptyFields = false;

      foreach (var header in headers)
      {
        try
        {
          ReadHeader(aRcFileContent, header);
          if (wasSaveStringsWithEmptyFields == false)
          {
            wasSaveStringsWithEmptyFields = true;
            SaveStringsWithEmptyFields(aRcFileContent);
          }
        }
        catch(FileNotFoundException fileNotFound)
        {
          MessageBox.Show(fileNotFound.Message);
        }
      }
    }

    #endregion

    #region Private methods

    private void SaveStringsWithEmptyFields(RCFileContent aRcFileContent)
    {
      var stringLines = aRcFileContent.GetStringLinesDictionary;
      foreach (var stringLine in stringLines)
        if (stringLine.Value.Id == ParseConstants.kIdDefaultValue)
          aRcFileContent.AddNewStringWithEmptyFields(stringLine.Value);
    }

    private void ReadHeader(RCFileContent aRcFileContent, string aHeaderFile)
    {
      if(!FileExists(aHeaderFile))
        throw new FileNotFoundException(aHeaderFile);

      using (StreamReader reader = new StreamReader(aHeaderFile))
        while (!reader.EndOfStream)
        {
          string line = reader.ReadLine();
          if (!line.Contains(TagConstants.kTagDefine))
            continue;

          List<string> lineElements = ParseUtility.BuildListOfStringsFromReadLine(line, kSplitResourceElementsChars);
          if (lineElements.Count < ParseConstants.kMinimumElementsToDefineString)
            continue;

          string idString = lineElements[2];
          idString = ExtractId(idString);

          int id = ParseConstants.kIdDefaultValue;
          if (ParseUtility.TransformToDecimal(idString, out id) == false)
            continue;

          if (!mHeaderContent.ContainString(lineElements[1]))
            mHeaderContent.AddElement(lineElements[1], id.ToString());

          if (aRcFileContent.ContainsLine(lineElements[1]))
            SaveString(aRcFileContent, lineElements[1], id);
        }
    }

    private string ExtractId(string aIdString)
    {
      while (mHeaderContent.ContainString(aIdString))
        aIdString = mHeaderContent.GetElement(aIdString);

      return aIdString;
    }

    private void SaveString(RCFileContent aRcFileContent, string aStringName, int aId)
    {
        aRcFileContent.InitId(aStringName, aId);
        InitStringTable(aRcFileContent, aStringName);
    }

    private void InitStringTable(RCFileContent aRcFileContent, string aName)
    {
      int stringTableNumber = aRcFileContent.GetStringTableNumber(aName);
      StringLine stringLine = aRcFileContent.GetStringLine(aName);

      if (aRcFileContent.ContainsStringTable(stringTableNumber))
        aRcFileContent.AddInStringTables(stringTableNumber, stringLine);
      else
      {
        aRcFileContent.AddNewStringTable(stringTableNumber, aRcFileContent.GetRcOrder(aName));
        aRcFileContent.AddInStringTables(stringTableNumber, stringLine);
      }
    }

    #endregion
  }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class HeaderFileWriter
  {
    #region Public methods

    public void WriteFile(RCFileContent aRcFileContent, string aPathHeaderFileRead, string aPathHeaderFileWrite)
    {
      using (StreamReader reader = new StreamReader(aPathHeaderFileRead))
        using (StreamWriter writer = new StreamWriter(aPathHeaderFileWrite))
          Write(reader, writer, aRcFileContent);
    }
    #endregion

    #region Private methods

    private void Write(StreamReader aReader, StreamWriter aWriter, RCFileContent aRcFileContent)
    {
      var stringLines = SortStringLinesAfterId(aRcFileContent);
      var iterator = stringLines.GetEnumerator();
      bool hasNext = iterator.MoveNext();

      while (!aReader.EndOfStream)
      {
        string line = aReader.ReadLine();
        List<string> lineElements = ParseUtility.BuildListOfStringsFromReadLine(line, Parse.kSplitResourceElementsChars);

        int numberPositionsToSkip = hasNext ? NumberPositionToSkip(aRcFileContent, iterator) : 0;
        while (numberPositionsToSkip-- > 0)
          hasNext = iterator.MoveNext();

        if (LineRepresentString(lineElements.Count, hasNext) == false || 
          !ParseUtility.TransformToDecimal(lineElements[2], out int stringId))
        {
          aWriter.WriteLine(line);
        }
        else if (CheckForAddedStrings(aWriter, iterator, line, stringId, aRcFileContent))
          hasNext = iterator.MoveNext();
      }
    }

    private List<KeyValuePair<string, StringLine>> SortStringLinesAfterId(RCFileContent aRcFileContent)
    {
      var stringLines = aRcFileContent.GetStringLinesDictionary.ToList();
      stringLines.Sort((pair1, pair2) => pair1.Value.Id.CompareTo(pair2.Value.Id));
      return stringLines;
    }

    //skip strings that are not in Resource.h and are in another header
    private int NumberPositionToSkip(RCFileContent aRcFileContent,
      IEnumerator<KeyValuePair<string, StringLine>> aIterator)
    {
      int countPositions = 0;
      bool hasEmptyFields;
      do
      {
        string name = aIterator.Current.Value.Name;
        hasEmptyFields = aRcFileContent.IsStringWithEmptyFields(name);
        if (hasEmptyFields)
          ++countPositions;
      } while (hasEmptyFields && aIterator.MoveNext());

      return countPositions;
    }

    private bool LineRepresentString(int aNumberOfLineElements, bool aHasNext) =>
      (aNumberOfLineElements < ParseConstants.kMinimumElementsToDefineString || aHasNext == false) ? false : true;
    
    private bool CheckForAddedStrings(StreamWriter aWriter, IEnumerator<KeyValuePair<string, StringLine>> aIterator, 
      string aLine, int aStringId, RCFileContent aRcFileContent)
    {
      bool skipOnePosition = true;
      if (aStringId < aIterator.Current.Value.Id)
        skipOnePosition = false;
      else if (aStringId > aIterator.Current.Value.Id && aRcFileContent.NewStringWasAdded == false) 
      {
        aRcFileContent.NewStringWasAdded = true;
        WriteAddedString(aWriter, aIterator);
      }
      aWriter.WriteLine(aLine);
      return skipOnePosition;
    }

    private void WriteAddedString(StreamWriter aWriter, IEnumerator<KeyValuePair<string, StringLine>> aIterator)
    {
      string emptySpaces = "                                        ";
      if ( TagConstants.kTagDefine.Length + aIterator.Current.Value.Name.Length < emptySpaces.Length -1)
      {
        emptySpaces = emptySpaces.Remove(0, TagConstants.kTagDefine.Length + aIterator.Current.Value.Name.Length);

        aWriter.WriteLine(TagConstants.kTagDefine + aIterator.Current.Value.Name +
          emptySpaces + aIterator.Current.Value.Id);
      }
      else
        aWriter.WriteLine(TagConstants.kTagDefine + aIterator.Current.Value.Name +
          " " + aIterator.Current.Value.Id);
    }
    #endregion
  }
}

using System.Collections.Generic;
using System.Text;

namespace StringEnhancer
{
  public class RCFileContentBuilder : IBuilder<RCFileContent>
  {
    private readonly string mPath;
    private readonly Encoding mCodePage;
    private readonly Dictionary<string, string> mNameToID;

    private RCFileContent mRCFileContent;

    public RCFileContentBuilder(string aPath, Encoding aCodePage, Dictionary<string, string> aNameToID)
    {
      mPath = aPath;
      mCodePage = aCodePage;
      mNameToID = aNameToID;
    }

    public void Build()
    {
      mRCFileContent = new RCFileContent()
      {
        StringTableContent = new Dictionary<int, List<RCFileItem>>(),
        StringTableIndexOrder = new List<int>()
      };

      ParseRCFile(mPath, mCodePage, mNameToID);
    }

    public RCFileContent GetResult() => mRCFileContent;

    private void ParseRCFile(string aPath, Encoding aCodePage, Dictionary<string, string> aNameToID)
    {
      using (var stringTableParser = new RCFileParser(aPath, aCodePage))
      {
        int currentStringTableIndex = Constants.kNotDiscovered; // Index of current STable
        int previousStringTableIndex = Constants.kNotDiscovered; // Index of previous STable

        int unusedSTableIndex = -100000;

        List<RCFileItem> unusedElements = new List<RCFileItem>();

        while (stringTableParser.HasNext())
        {
          var obj = stringTableParser.GetNext();

          if (obj.Name == "BEGIN") continue;

          if (obj.Name == "END") // End of the current STable
          {
            if (currentStringTableIndex != Constants.kNotDiscovered && previousStringTableIndex != currentStringTableIndex) // If index has been calculated and it is new
            {
              mRCFileContent.StringTableIndexOrder.Add(currentStringTableIndex); // Add current index to the order list

              // Add unused to the beginning
              mRCFileContent.StringTableContent[currentStringTableIndex].InsertRange(0, unusedElements);
            }
            else if (currentStringTableIndex == Constants.kNotDiscovered)
            {
              // Assign a meaningless STable index
              currentStringTableIndex = unusedSTableIndex++;
              // Add to StringTableIndexOrder
              mRCFileContent.StringTableIndexOrder.Add(currentStringTableIndex);
              // Add all unused to STable
              mRCFileContent.StringTableContent.Add(currentStringTableIndex, new List<RCFileItem>());
              mRCFileContent.StringTableContent[currentStringTableIndex].AddRange(unusedElements);
            }

            unusedElements.Clear();
            previousStringTableIndex = currentStringTableIndex;
            currentStringTableIndex = Constants.kNotDiscovered; // Reset index of current STable
          }
          else // Found new object from current STable
          {
            if (!aNameToID.ContainsKey(obj.Name)) // If there's no ID for the current Name in current RC's headers
            {
              if (currentStringTableIndex == Constants.kNotDiscovered)
                unusedElements.Add(obj);
              obj.ID = Constants.kNotFoundID;
            }
            else
            {
              obj.ID = aNameToID[obj.Name];
            }

            if (currentStringTableIndex == Constants.kNotDiscovered && obj.ID != Constants.kNotFoundID) // If index is not yet calculated and ID exists
            {
              currentStringTableIndex = StringTableIndexCalculator.CalculateIndex(aNameToID[obj.Name]); // Get index of current STable
            }

            if (currentStringTableIndex == Constants.kNotDiscovered) continue; // Could not calculate index of current STable

            if (!mRCFileContent.StringTableContent.ContainsKey(currentStringTableIndex)) // In case current STable is not registered in the Dictionary
            {
              mRCFileContent.StringTableContent.Add(currentStringTableIndex, new List<RCFileItem>()); // Register STable in Dictionary
            }
            mRCFileContent.StringTableContent[currentStringTableIndex].Add(obj); // Add current object to its STable
          }
        }
      }
    }
  }
}
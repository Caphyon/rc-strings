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
        int currentStringTableIndex = -1; // Index of current STable
        int previousStringTableIndex = -1; // Index of previous STable

        while (stringTableParser.HasNext())
        {
          var obj = stringTableParser.GetNext();

          if (obj.Name == "BEGIN") continue;

          if (obj.Name == "END") // End of the current STable
          {
            if (currentStringTableIndex != -1 && previousStringTableIndex != currentStringTableIndex) // If index has been calculated and it is new
            {
              mRCFileContent.StringTableIndexOrder.Add(currentStringTableIndex); // Add current index to the order list
            }

            previousStringTableIndex = currentStringTableIndex;
            currentStringTableIndex = -1; // Reset index of current STable
          }
          else // Found new object from current STable
          {
            if (!aNameToID.ContainsKey(obj.Name)) continue;  // If there's no ID for the current Name in Header file, skip it
            else
            {
              obj.ID = aNameToID[obj.Name];
            }

            if (currentStringTableIndex == -1) // If index is not yet calculated
            {
              currentStringTableIndex = int.Parse(aNameToID[obj.Name]) / Constants.kStringTableCapacity; // Get index of current STable
            }

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
using System.Collections.Generic;

namespace StringEnhancer
{
  public class HeaderContent
  {
    // List of all the objects from Header file
    public Dictionary<string, List<HeaderItem>> SortedHeaderResults { get; set; } = new Dictionary<string, List<HeaderItem>>();
    // Dict to efficiently lookup the ID of a given Name (key = name, val = ID)
    public Dictionary<string, HeaderId> NameToID { get; set; } = new Dictionary<string, HeaderId>();

    public int GetPosition(HeaderId aID, string aHeaderPath)
    {
      var searchedItem = new HeaderItem { ID = IDNormalizer.CopyNormalizeHexaID(aID) };

      if (!SortedHeaderResults.ContainsKey(aHeaderPath))
        SortedHeaderResults.Add(aHeaderPath, new List<HeaderItem>());
      var foundIndex = SortedHeaderResults[aHeaderPath].BinarySearch(0, SortedHeaderResults[aHeaderPath].Count, searchedItem, new HeaderResultComparerByID());

      if (foundIndex >= 0) return Constants.kDuplicateID;
      else return ~foundIndex;
    }
  }
}

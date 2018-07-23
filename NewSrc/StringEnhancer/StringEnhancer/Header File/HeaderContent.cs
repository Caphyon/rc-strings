using System.Collections.Generic;

namespace StringEnhancer
{
  public class HeaderContent
  {
    public Dictionary<string, List<HeaderItem>> SortedHeaderResults { get; set; } // List of all the objects from Header file
    public Dictionary<string, string> NameToID { get; set; } // Dict to efficiently lookup the ID of a given Name (key = name, val = ID)

    public int GetPosition(string aID, string aHeaderPath)
    {
      var searchedItem = new HeaderItem { ID = aID };

      var foundIndex = SortedHeaderResults[aHeaderPath].BinarySearch(0, SortedHeaderResults[aHeaderPath].Count, searchedItem, new HeaderResultComparerByID());

      if (foundIndex >= 0) return Constants.kDuplicateID;
      else return ~foundIndex;
    }
  }
}

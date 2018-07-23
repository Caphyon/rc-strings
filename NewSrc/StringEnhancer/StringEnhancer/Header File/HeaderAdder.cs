using System;

namespace StringEnhancer
{
  public static class HeaderAdder
  {
    public static int AddItem(TestItem aTestItem, HeaderContent aHeaderContent, string aHeaderPath)
    {
      var nameToID = aHeaderContent.NameToID;
      var sortedHeaderResults = aHeaderContent.SortedHeaderResults;

      // Check Name Unique
      if (nameToID.ContainsKey(aTestItem.Name)) return Constants.kDuplicateID;

      // Check ID Unique
      var insertedItem = new HeaderItem { ID = aTestItem.ID, Name = aTestItem.Name };

      var foundIndex = aHeaderContent.GetPosition(insertedItem.ID, aHeaderPath);
      if (foundIndex == Constants.kDuplicateID) return foundIndex;

      sortedHeaderResults[aHeaderPath].Insert(foundIndex, insertedItem);
      nameToID[aTestItem.Name] = aTestItem.ID;

      return foundIndex;
    }
  }
}
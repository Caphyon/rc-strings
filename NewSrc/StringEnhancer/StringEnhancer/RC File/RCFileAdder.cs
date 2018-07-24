using System;
using System.Collections.Generic;

namespace StringEnhancer
{
  public static class RCFileAdder
  {
    public static void AddItem(TestItem aTestItem,
      RCFileContent aRCFileContent, Dictionary<string, string> aNameToID)
    {
      var stringTableContent = aRCFileContent.StringTableContent;
      var stringTableIndexOrder = aRCFileContent.StringTableIndexOrder;
      var determinedStringTableIndex = StringTableIndexCalculator.CalculateIndex(aTestItem.ID);

      // Check if StringTable with index currentIdx exists
      if (!stringTableContent.ContainsKey(determinedStringTableIndex))
      {
        stringTableContent.Add(determinedStringTableIndex, new List<RCFileItem>());
        stringTableIndexOrder.Add(determinedStringTableIndex);
      }

      var printStyle = StringTablePrintStyleDeterminer.DeterminePrintStyle(aTestItem.Name, aTestItem.Value);

      bool notYetAdded = true;

      var startIndex = (stringTableContent[determinedStringTableIndex].Count / Constants.kStringTableCapacity) * Constants.kStringTableCapacity;

      int.TryParse(aTestItem.ID, out var testItemID);

      for (int i = startIndex; i < stringTableContent[determinedStringTableIndex].Count; ++i)
      {
        var currentName = stringTableContent[determinedStringTableIndex][i].Name;
        aNameToID.TryGetValue(currentName, out var currentIDString);
        int.TryParse(currentIDString, out var currentID);

        if (testItemID < currentID)
        {
          stringTableContent[determinedStringTableIndex].Insert(i,
          new RCFileItem
          {
            Name = aTestItem.Name,
            Value = aTestItem.Value,
            PrintStyle = printStyle
          });

          notYetAdded = false;
          break;
        }
      }

      if (notYetAdded)
      {
        stringTableContent[determinedStringTableIndex].Add(
        new RCFileItem
        {
          Name = aTestItem.Name,
          Value = aTestItem.Value,
          PrintStyle = printStyle
        });

        notYetAdded = false;
      }
    }
  }
}

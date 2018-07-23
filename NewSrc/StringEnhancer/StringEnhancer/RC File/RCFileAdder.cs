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

      for (int i = startIndex; i < stringTableContent[determinedStringTableIndex].Count; ++i)
      {
        var currentName = stringTableContent[determinedStringTableIndex][i].Name;
        var currentID = Convert.ToInt32(aNameToID[currentName]);

        if (Convert.ToInt32(aTestItem.ID) < currentID)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringEnhancer
{
  public static class RCFileEditor
  {
    public static void EditValue(string aCurrentName, string aNewValue,
      RCFileContent aRCFileContent, HeaderContent aHeaderContent)
    {
      if (!aHeaderContent.NameToID.ContainsKey(aCurrentName)) return;

      var stringTableContent = aRCFileContent.StringTableContent;
      var stringTableDeterminedIndex = StringTableIndexCalculator.CalculateIndex(aHeaderContent.NameToID[aCurrentName]);

      foreach (var rcItem in stringTableContent[stringTableDeterminedIndex])
      {
        if (rcItem.Name == aCurrentName)
        {
          rcItem.Value = aNewValue;
          return;
        }
      }
    }
  }
}

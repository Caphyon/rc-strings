using System.Collections.Generic;

namespace StringEnhancer
{
  public class RCFileContent
  {
    public Dictionary<int, List<RCFileItem>> StringTableContent { get; set; } // Store elements of STable based on index (key = index, val = list of elements)
    public List<int> StringTableIndexOrder { get; set; } // Stores order of STables based on indices

    public RCFileItem GetStringLineForName(string aStringResourceName, string aStringResourceID)
    {
      var stringTableIndex = StringTableIndexCalculator.CalculateIndex(aStringResourceID);

      if (!StringTableContent.ContainsKey(stringTableIndex)) return null;

      foreach (var element in StringTableContent[stringTableIndex])
      {
        if (element.Name == aStringResourceName)
        {
          return element;
        }
      }

      return null;
    }
  }
}
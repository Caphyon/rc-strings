using System.Collections.Generic;

namespace StringEnhancer
{
  public class RCFileContent
  {
    // Store elements of STable based on index (key = index, val = list of elements)
    public Dictionary<int, List<RCFileItem>> StringTableContent { get; set; } = new Dictionary<int, List<RCFileItem>>();
    // Stores order of STables based on indices
    public List<int> StringTableIndexOrder { get; set; } = new List<int>();

    public RCFileItem GetStringLineForName(string aStringResourceName, HeaderId aStringResourceID)
    {
      if (aStringResourceID == Constants.kNotFoundID)
      {
        foreach (var idx in StringTableContent.Keys)
        {
          foreach (var element in StringTableContent[idx])
          {
            if (element.Name == aStringResourceName)
            {
              return element;
            }
          }
        }
      }
      else
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
      }

      return null;
    }
  }
}
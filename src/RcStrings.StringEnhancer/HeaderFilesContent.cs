using System.Collections.Generic;
using System.Linq;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class HeaderFilesContent
  {
    #region Members
    private Dictionary<string, string> mHeaderElements = new Dictionary<string, string>();
    #endregion

    #region Public methods

    public void AddElement(string aName, string aId) => mHeaderElements.Add(aName, aId);

    public bool ContainString(string aName) => mHeaderElements.ContainsKey(aName);

    public string GetElement(string aName) => mHeaderElements[aName];

    public List<KeyValuePair<string, string>> SortByIdValue()
    {
      var elements = mHeaderElements.ToList();
      elements.Sort((pair1, pair2) =>
      {
        ParseUtility.TransformToDecimal(pair1.Value, out int id1);
        ParseUtility.TransformToDecimal(pair2.Value, out int id2);
        return id1.CompareTo(id2);
      });
      return elements;
    }
    #endregion
  }
}

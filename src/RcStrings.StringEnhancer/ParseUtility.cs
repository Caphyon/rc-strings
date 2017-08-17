using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class ParseUtility
  {
    #region Public methods

    static public List<string> BuildListOfStringsFromReadLine(string aReadLine, char[] aDelimiters) =>
      aReadLine.Split(aDelimiters)
        .Where(l => l != string.Empty)
        .Select(l => l.Trim())
        .ToList();

    static public bool TransformToDecimal(string aIdString, out int aId) =>
      aIdString.Contains("0x") == true ? int.TryParse(aIdString.Substring(2), NumberStyles.HexNumber,
          CultureInfo.InvariantCulture, out aId) : int.TryParse(aIdString, out aId);

    #endregion
  }
}

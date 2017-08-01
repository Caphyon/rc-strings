using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class ParseUtility
  {
    static public List<string> BuildListOfStringsFromReadLine(string aReadLine, char[] aDelimiters)
    {
      List<string> lineElements = aReadLine.Split(aDelimiters)
              .Where(l => l != string.Empty)
              .Select(l => l.Trim())
              .ToList();

      return lineElements;
    }

    static public bool TransformToDecimal(string aIdString, out int aId) =>
      aIdString.Contains("0x") == true ? int.TryParse(aIdString.Substring(2), NumberStyles.HexNumber,
          CultureInfo.InvariantCulture, out aId) : int.TryParse(aIdString, out aId);
  }
}

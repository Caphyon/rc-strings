using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class StringTable
  {
    public int StringTableNumber { get; private set; }  // string id / 16
    public int RcOrder { get; private set; }
    public int ElementsCount { get; private set; }

    private StringLine[] mStringLinesArray = new StringLine[ParseConstants.kMaximumNumberOfStringsInStringTable];

    public StringTable(int aStringTableNumber, int aRcOrder)
    {
      RcOrder = aRcOrder;
      StringTableNumber = aStringTableNumber;
      ElementsCount = 0;
    }

    // id % 16 = position in stringLinesArray 
    public void AddInformation(StringLine aStringLine)
    {
      ElementsCount = ElementsCount + 1;
      mStringLinesArray[aStringLine.Id % ParseConstants.kMaximumNumberOfStringsInStringTable] = aStringLine;
    }

    public bool IsPositionEmpty(int aPosition) => mStringLinesArray[aPosition] == null;

    public void Display(StreamWriter aStreamWriter)
    {
      foreach (var stringLine in mStringLinesArray.Where(item => item != null))
        aStreamWriter.WriteLine(stringLine);
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class OperationsStringTable
  {
    private RCFileContent mRcFileContent;
    private EmptyRangeManager mEmptyRangeManager;

    public OperationsStringTable(RCFileContent aRcFileContent, EmptyRangeManager aEmptyRangeManager)
    {
      mRcFileContent = aRcFileContent;
      mEmptyRangeManager = aEmptyRangeManager;
    }

    public StringLine AddStringResource(string aValue, string aName, int aId)
    {
      StringLine stringLine = new StringLine(aName, aValue, aId);
      if (aId % ParseConstants.kMaximumNumberOfStringsInStringTable == 0)
        stringLine.RcOrder = mRcFileContent.GetStringLinesDictionary.Count + 1;

      if ( !mRcFileContent.ExistsStringTable(aId / ParseConstants.kMaximumNumberOfStringsInStringTable))
        mRcFileContent.AddNewStringTable(aId / ParseConstants.kMaximumNumberOfStringsInStringTable,
            mRcFileContent.StringTablesDictionary.Count());

      SaveString(stringLine);
      return stringLine;
    }

    private void SaveString(StringLine aStringLine)
    {
      mRcFileContent.AddInStringLines(aStringLine);
      mRcFileContent.AddInStringTables(aStringLine.Id / ParseConstants.kMaximumNumberOfStringsInStringTable, aStringLine);
    }

  }
}

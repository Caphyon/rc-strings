using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class OperationsStringTable
  {
    #region Members

    private RCFileContent mRcFileContent;
    private EmptyRangeManager mEmptyRangeManager;
    #endregion

    #region Ctor

    public OperationsStringTable(RCFileContent aRcFileContent, EmptyRangeManager aEmptyRangeManager)
    {
      mRcFileContent = aRcFileContent;
      mEmptyRangeManager = aEmptyRangeManager;
    }
    #endregion

    #region Public methods

    public StringLine AddStringResource(string aValue, string aName, int aId)
    {
      StringLine stringLine = new StringLine(aName, aValue, aId);
      if (aId % ParseConstants.kStringTableCapacity == 0)
        stringLine.RcOrder = mRcFileContent.GetStringLinesDictionary.Count + 1;

      if ( !mRcFileContent.ExistsStringTable(aId / ParseConstants.kStringTableCapacity))
        mRcFileContent.AddNewStringTable(aId / ParseConstants.kStringTableCapacity,
            mRcFileContent.StringTablesDictionary.Count());

      SaveString(stringLine);
      return stringLine;
    }
    #endregion

    #region Private methods

    private void SaveString(StringLine aStringLine)
    {
      mRcFileContent.AddInStringLines(aStringLine);
      mRcFileContent.AddInStringTables(aStringLine.Id / ParseConstants.kStringTableCapacity, aStringLine);
    }
    #endregion
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public static class ParseConstants
  {
    #region Constants

    public const int kMaximumLengthForMoreSpacesAfterName = 22;
    public const int kMaximumLengthToWriteASingleLine = 64;

    public const int kMaximumResourceNameLength = 247;
    public const int kMaximumResourceValueLength = 4096;

    public const int kIdDefaultValue = -1;
    public const int kRcOrderDefaultValue = -1;

    public const int kStringTableCapacity = 16;
    public const int kMinimumElementsToDefineString = 3;

    public const int kNumberOfWordsInStringName = 3;
    public const int kMinimumRelevantWordLength = 4;

    #endregion
  }
}

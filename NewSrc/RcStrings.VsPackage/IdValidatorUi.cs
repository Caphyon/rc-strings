using System;
using StringEnhancer;

namespace Caphyon.RcStrings.VsPackage
{
  public static class IdValidatorUi
  {
    public static bool IsInValidRange(HeaderId aID) =>
      IsInValidRange(aID.Value);
    public static bool IsInValidRange(string aString)
    {
      int intRepr;
      if (IDValidator.IsHexaRepresentation(aString)) intRepr = Convert.ToInt32(aString, 16);
      else if (IDValidator.IsValidInteger(aString)) intRepr = int.Parse(aString);
      else return false;

      return (intRepr >= UiConstants.kMinId && intRepr <= UiConstants.kMaxId);
    }
  }
}

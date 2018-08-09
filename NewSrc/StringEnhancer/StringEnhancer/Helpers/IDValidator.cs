using System;
using System.Linq;

namespace StringEnhancer
{
  public static class IDValidator
  {
    public static bool IsValid(HeaderId aID, HeaderContent aHeaderContent) =>
      IsValid(aID.Value, aHeaderContent);
    public static bool IsValid(string aString, HeaderContent aHeaderContent) =>
      !IsRecurrentCase(aString, aHeaderContent) && IsValidWithoutRecurrenceCheck(aString) || aString == Constants.kNotFoundID.Value;

    public static bool IsValidWithoutRecurrenceCheck(HeaderId aID) =>
      IsValidWithoutRecurrenceCheck(aID.Value);
    public static bool IsValidWithoutRecurrenceCheck(string aString) =>
      !IsEmpty(aString) && aString != Constants.kInvalidID.Value && IsInValidRange(aString) && (IsHexaRepresentation(aString) || IsValidInteger(aString));

    public static bool IsEmpty(HeaderId aID) => IsEmpty(aID.Value);
    public static bool IsEmpty(string aString) => (aString.Length == 0);

    public static bool IsValidInteger(HeaderId aID) =>
      IsValidInteger(aID.Value);
    public static bool IsValidInteger(string aString) =>
      int.TryParse(aString, out _);

    public static bool IsInValidRange(HeaderId aID) =>
      IsInValidRange(aID.Value);
    public static bool IsInValidRange(string aString)
    {
      int intRepr;
      if (IsHexaRepresentation(aString)) intRepr = Convert.ToInt32(aString, 16);
      else if (IsValidInteger(aString)) intRepr = int.Parse(aString);
      else return false;

      return (intRepr >= Constants.kMinID && intRepr <= Constants.kMaxID);
    }

    public static bool IsRecurrentCase(HeaderId aID, HeaderContent aHeaderContent) =>
      IsRecurrentCase(aID.Value, aHeaderContent);
    public static bool IsRecurrentCase(string aString, HeaderContent aHeaderContent) =>
      !IsValidWithoutRecurrenceCheck(aString) && aHeaderContent.NameToID.ContainsKey(aString);

    public static bool IsHexaRepresentation(HeaderId aID) =>
      IsHexaRepresentation(aID.Value);
    public static bool IsHexaRepresentation(string aString) =>
      aString.Length == 6 && aString.StartsWith("0x") && aString.Substring(2).All(IsHexaCharacter);

    public static bool IsHexaCharacter(char aChar) =>
      (aChar >= '0' && aChar <= '9') || (aChar >= 'A' && aChar <= 'F');
  }
}

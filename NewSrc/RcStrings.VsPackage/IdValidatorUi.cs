using System;
using StringEnhancer;

namespace Caphyon.RcStrings.VsPackage
{
  public class IdValidatorUi
  {
    public string Validate(StringResourceContext aResourceContext, string aResourceIdTemp, HeaderId aResourceId)
    {
      if (string.IsNullOrEmpty(aResourceIdTemp))
      {
        return "ID can not be empty.";
      }

      if (aResourceId.Value == StringEnhancer.Constants.kInvalidID.Value || !IsInValidRange(aResourceId))
      {
        if (IDValidator.IsValidInteger(aResourceIdTemp) || IDValidator.IsHexaRepresentation(aResourceIdTemp) && !IsInValidRange(aResourceIdTemp))
          return $"ID can not be less than {UiConstants.kMinId} or greater than {UiConstants.kMaxId}.";
        else
          return "Integer or hexadecimal format is required.";
      }

      if (aResourceContext.IdExists(aResourceId))
      {
        return "Another resource with this ID already exists.";
      }

      return null;
    }

    public bool IsInValidRange(HeaderId aId) =>
      IsInValidRange(aId.Value);
    public bool IsInValidRange(string aString)
    {
      int intRepr;
      if (IDValidator.IsHexaRepresentation(aString)) intRepr = Convert.ToInt32(aString, 16);
      else if (IDValidator.IsValidInteger(aString)) intRepr = int.Parse(aString);
      else return false;

      return (intRepr >= UiConstants.kMinId && intRepr <= UiConstants.kMaxId);
    }
  }
}

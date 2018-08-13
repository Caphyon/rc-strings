using System.Collections.Generic;
using StringEnhancer;

namespace Caphyon.RcStrings.VsPackage
{
  public class NameValidatorUi
  {
    public string Validate(StringResourceContext aResourceContext, string aResourceName)
    {
      if (string.IsNullOrEmpty(aResourceName))
      {
        return "Name can not be empty.";
      }

      if (aResourceName.Contains(" "))
      {
        return "Name can not contain whitespaces.";
      }

      if (aResourceName.Length > ParseConstants.kMaximumResourceNameLength)
      {
        return $"Name can not be longer than {ParseConstants.kMaximumResourceNameLength} characters.";
      }

      if (aResourceContext.ResourceNameExists(aResourceName))
      {
        return "Another resource with this name already exists.";
      }

      return null;
    }
  }
}
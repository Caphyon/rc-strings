using StringEnhancer;

namespace Caphyon.RcStrings.VsPackage
{
  public class ValueValidatorUi
  {
    public string Validate(string aResourceValue)
    {
      if (aResourceValue.Length > ParseConstants.kMaximumResourceValueLength)
      {
        return $"Value can not be longer than {ParseConstants.kMaximumResourceValueLength} characters.";
      }

      return null;
    }
  }
}
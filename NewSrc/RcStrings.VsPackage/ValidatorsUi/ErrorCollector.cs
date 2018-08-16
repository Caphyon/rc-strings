using System;
using System.Collections.Generic;
using System.Linq;
using StringEnhancer;

namespace Caphyon.RcStrings.VsPackage
{
  public class ErrorCollector
  {
    #region Private members
    private Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();
    private NameValidatorUi mNameValidator =  new NameValidatorUi();
    private IdValidatorUi mIdValidator = new IdValidatorUi();
    private ValueValidatorUi mValueValidator = new ValueValidatorUi();
    #endregion

    #region Specific methods
    public bool HasAnyErrors() => Errors.Any();

    public string HasError(string aPropertyName)
    {
      return Errors.ContainsKey(aPropertyName) ?
        string.Join("\n", Errors[aPropertyName]) : string.Empty;
    }

    public void Clear() => Errors.Clear();

    public KeyValuePair<string, List<string>> First()
    {
      return Errors.First();
    }
    #endregion

    #region Checking methods
    public void CheckName(StringResourceContext aResourceContext, string aResourceName)
    {
      var errorMessage = mNameValidator.Validate(aResourceContext, aResourceName);
      if (errorMessage == null) return;

      var propertyName = "ResourceName";

      if (!Errors.ContainsKey(propertyName))
        Errors[propertyName] = new List<string>();

      Errors[propertyName].Add(errorMessage);
    }

    public void CheckId(StringResourceContext aResourceContext, string aResourceIdTemp, HeaderId aResourceId)
    {
      var errorMessage = mIdValidator.Validate(aResourceContext, aResourceIdTemp, aResourceId);
      if (errorMessage == null) return;

      var propertyName = "ResourceIdTemp";

      if (!Errors.ContainsKey(propertyName))
        Errors[propertyName] = new List<string>();

      Errors[propertyName].Add(errorMessage);
    }

    public void CheckValue(string aResourceValue)
    {
      var errorMessage = mValueValidator.Validate(aResourceValue);
      if (errorMessage == null) return;

      var propertyName = "ResourceValue";

      if (!Errors.ContainsKey(propertyName))
        Errors[propertyName] = new List<string>();

      Errors[propertyName].Add(errorMessage);
    }
    #endregion
  }
}
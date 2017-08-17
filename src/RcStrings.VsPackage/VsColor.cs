using System.Windows;

namespace Caphyon.RcStrings
{
  public class VsColor : DynamicResourceExtension
  {
    #region Members
    private const string kClassPrefix = "VsColor";
    #endregion

    #region Ctor

    public VsColor(object resourceKey)
    {
      string key = string.Format("{0}.{1}", kClassPrefix, resourceKey.ToString());
      this.ResourceKey = key.Substring(0, key.Length - 3);
    }
    #endregion
  }
}

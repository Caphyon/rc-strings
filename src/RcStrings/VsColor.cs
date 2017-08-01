using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Caphyon.RcStrings
{
  public class VsColor : DynamicResourceExtension
  {
    private const string kClassPrefix = "VsColor";

    public VsColor(object resourceKey)
    {
      string key = string.Format("{0}.{1}", kClassPrefix, resourceKey.ToString());
      this.ResourceKey = key.Substring(0, key.Length - 3);
    }
  }
}

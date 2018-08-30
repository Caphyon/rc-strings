using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Caphyon.RcStrings.VsPackage
{
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class VisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var hasError = (bool)value;
      return hasError ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

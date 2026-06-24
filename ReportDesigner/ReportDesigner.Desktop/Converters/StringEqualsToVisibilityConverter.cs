using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReportDesigner.Desktop.Converters;

[ValueConversion(typeof(string), typeof(Visibility))]
public class StringEqualsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? strValue = value?.ToString();
        string? compareTo = parameter?.ToString();
        return strValue == compareTo ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

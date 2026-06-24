using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReportDesigner.Desktop.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;
        string trueBrush = parameter?.ToString() ?? "#FFE0E0E0";

        try
        {
            return boolValue 
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(trueBrush))
                : Brushes.Transparent;
        }
        catch
        {
            return Brushes.Transparent;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

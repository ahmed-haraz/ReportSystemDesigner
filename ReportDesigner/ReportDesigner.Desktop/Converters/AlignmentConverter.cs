using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TextAlignment = ReportDesigner.Core.Models.TextAlignment;

namespace ReportDesigner.Desktop.Converters;

public class AlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TextAlignment alignment) return TextAlignment.Left;

        return alignment switch
        {
            TextAlignment.Center => HorizontalAlignment.Center,
            TextAlignment.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

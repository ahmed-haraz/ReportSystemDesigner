using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReportDesigner.Desktop.Converters;

public class StringToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str) return Brushes.Black;

        try
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(str));
        }
        catch
        {
            return Brushes.Black;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

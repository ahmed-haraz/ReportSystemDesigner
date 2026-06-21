using CommunityToolkit.Mvvm.ComponentModel;
using ReportDesigner.Core.Models;

namespace ReportDesigner.Desktop.ViewModels;

public partial class PropertiesViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportObject? _selectedObject;

    [ObservableProperty]
    private Band? _selectedBand;

    partial void OnSelectedObjectChanged(ReportObject? value)
    {
        if (value != null)
        {
            ObjectLeft = value.Left;
            ObjectTop = value.Top;
            ObjectWidth = value.Width;
            ObjectHeight = value.Height;
            ObjectName = value.Name;
            ObjectText = value.Text;
            ObjectDataBinding = value.DataBinding;
            ObjectExpression = value.Expression;
        }
    }

    [ObservableProperty]
    private float _objectLeft;

    [ObservableProperty]
    private float _objectTop;

    [ObservableProperty]
    private float _objectWidth;

    [ObservableProperty]
    private float _objectHeight;

    [ObservableProperty]
    private string _objectName = "";

    [ObservableProperty]
    private string _objectText = "";

    [ObservableProperty]
    private string _objectDataBinding = "";

    [ObservableProperty]
    private string _objectExpression = "";

    [ObservableProperty]
    private string _objectFontName = "Arial";

    [ObservableProperty]
    private float _objectFontSize = 10;

    [ObservableProperty]
    private bool _objectBold;

    [ObservableProperty]
    private bool _objectItalic;

    [ObservableProperty]
    private string _objectForeColor = "#000000";

    [ObservableProperty]
    private string _objectBackColor = "#FFFFFF";

    [ObservableProperty]
    private TextAlignment _objectAlignment = TextAlignment.Left;

    [ObservableProperty]
    private bool _objectBorderLeft;

    [ObservableProperty]
    private bool _objectBorderRight;

    [ObservableProperty]
    private bool _objectBorderTop;

    [ObservableProperty]
    private bool _objectBorderBottom;
}

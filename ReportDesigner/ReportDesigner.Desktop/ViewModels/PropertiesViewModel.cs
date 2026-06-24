using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;

namespace ReportDesigner.Desktop.ViewModels;

public partial class PropertiesViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportObject? _selectedObject;

    [ObservableProperty]
    private Band? _selectedBand;

    public bool HasSelectedObject => SelectedObject != null;

    // Position properties
    [ObservableProperty]
    private float _objectLeft;

    [ObservableProperty]
    private float _objectTop;

    [ObservableProperty]
    private float _objectWidth;

    [ObservableProperty]
    private float _objectHeight;

    // Identity properties
    [ObservableProperty]
    private string _objectName = "";

    [ObservableProperty]
    private string _objectText = "";

    [ObservableProperty]
    private string _objectDataBinding = "";

    [ObservableProperty]
    private string _objectExpression = "";

    // Text properties
    [ObservableProperty]
    private string _objectFontName = "Arial";

    [ObservableProperty]
    private float _objectFontSize = 10;

    [ObservableProperty]
    private bool _objectBold;

    [ObservableProperty]
    private bool _objectItalic;

    [ObservableProperty]
    private bool _objectUnderline;

    [ObservableProperty]
    private string _objectForeColor = "#000000";

    [ObservableProperty]
    private string _objectBackColor = "#FFFFFF";

    [ObservableProperty]
    private TextAlignment _objectAlignment = TextAlignment.Left;

    [ObservableProperty]
    private bool _objectWordWrap = true;

    [ObservableProperty]
    private string _objectFormatString = "";

    // Border properties
    [ObservableProperty]
    private bool _objectBorderLeft;

    [ObservableProperty]
    private bool _objectBorderRight;

    [ObservableProperty]
    private bool _objectBorderTop;

    [ObservableProperty]
    private bool _objectBorderBottom;

    [ObservableProperty]
    private float _objectBorderWidth = 1;

    [ObservableProperty]
    private string _objectBorderColor = "#000000";

    // Behavior properties
    [ObservableProperty]
    private bool _objectCanGrow = true;

    [ObservableProperty]
    private bool _objectCanShrink;

    [ObservableProperty]
    private bool _objectVisible = true;

    partial void OnSelectedObjectChanged(ReportObject? value)
    {
        OnPropertyChanged(nameof(HasSelectedObject));

        if (value != null)
        {
            LoadObjectProperties(value);
        }
        else
        {
            ClearObjectProperties();
        }
    }

    partial void OnObjectLeftChanged(float value) => UpdateObjectProperty(o => o.Left = value);
    partial void OnObjectTopChanged(float value) => UpdateObjectProperty(o => o.Top = value);
    partial void OnObjectWidthChanged(float value) => UpdateObjectProperty(o => o.Width = value);
    partial void OnObjectHeightChanged(float value) => UpdateObjectProperty(o => o.Height = value);
    partial void OnObjectNameChanged(string value) => UpdateObjectProperty(o => o.Name = value);
    partial void OnObjectTextChanged(string value) => UpdateObjectProperty(o => o.Text = value);
    partial void OnObjectDataBindingChanged(string value) => UpdateObjectProperty(o => o.DataBinding = value);
    partial void OnObjectExpressionChanged(string value) => UpdateObjectProperty(o => o.Expression = value);
    partial void OnObjectFontNameChanged(string value) => UpdateTextProperty(t => t.FontName = value);
    partial void OnObjectFontSizeChanged(float value) => UpdateTextProperty(t => t.FontSize = value);
    partial void OnObjectBoldChanged(bool value) => UpdateTextProperty(t => t.Bold = value);
    partial void OnObjectItalicChanged(bool value) => UpdateTextProperty(t => t.Italic = value);
    partial void OnObjectUnderlineChanged(bool value) => UpdateTextProperty(t => t.Underline = value);
    partial void OnObjectForeColorChanged(string value) => UpdateTextProperty(t => t.ForeColor = value);
    partial void OnObjectBackColorChanged(string value) => UpdateTextProperty(t => t.BackColor = value);
    partial void OnObjectAlignmentChanged(TextAlignment value) => UpdateTextProperty(t => t.Alignment = value);
    partial void OnObjectWordWrapChanged(bool value) => UpdateTextProperty(t => t.WordWrap = value);
    partial void OnObjectFormatStringChanged(string value) => UpdateObjectProperty(o => o.FormatString = value);
    partial void OnObjectBorderLeftChanged(bool value) => UpdateBorderProperty(b => b.Left = value);
    partial void OnObjectBorderRightChanged(bool value) => UpdateBorderProperty(b => b.Right = value);
    partial void OnObjectBorderTopChanged(bool value) => UpdateBorderProperty(b => b.Top = value);
    partial void OnObjectBorderBottomChanged(bool value) => UpdateBorderProperty(b => b.Bottom = value);
    partial void OnObjectBorderWidthChanged(float value) => UpdateBorderProperty(b => b.Width = value);
    partial void OnObjectBorderColorChanged(string value) => UpdateBorderProperty(b => b.Color = value);
    partial void OnObjectCanGrowChanged(bool value) => UpdateObjectProperty(o => o.CanGrow = value);
    partial void OnObjectCanShrinkChanged(bool value) => UpdateObjectProperty(o => o.CanShrink = value);
    partial void OnObjectVisibleChanged(bool value) => UpdateObjectProperty(o => o.Visible = value);

    public void LoadObjectProperties(ReportObject obj)
    {
        ObjectLeft = obj.Left;
        ObjectTop = obj.Top;
        ObjectWidth = obj.Width;
        ObjectHeight = obj.Height;
        ObjectName = obj.Name;
        ObjectText = obj.Text;
        ObjectDataBinding = obj.DataBinding;
        ObjectExpression = obj.Expression;
        ObjectFormatString = obj.FormatString;
        ObjectCanGrow = obj.CanGrow;
        ObjectCanShrink = obj.CanShrink;
        ObjectVisible = obj.Visible;

        if (obj.TextProps != null)
        {
            ObjectFontName = obj.TextProps.FontName;
            ObjectFontSize = obj.TextProps.FontSize;
            ObjectBold = obj.TextProps.Bold;
            ObjectItalic = obj.TextProps.Italic;
            ObjectUnderline = obj.TextProps.Underline;
            ObjectForeColor = obj.TextProps.ForeColor;
            ObjectBackColor = obj.TextProps.BackColor;
            ObjectAlignment = obj.TextProps.Alignment;
            ObjectWordWrap = obj.TextProps.WordWrap;
        }

        if (obj.Border != null)
        {
            ObjectBorderLeft = obj.Border.Left;
            ObjectBorderRight = obj.Border.Right;
            ObjectBorderTop = obj.Border.Top;
            ObjectBorderBottom = obj.Border.Bottom;
            ObjectBorderWidth = obj.Border.Width;
            ObjectBorderColor = obj.Border.Color;
        }
    }

    private void ClearObjectProperties()
    {
        ObjectLeft = 0;
        ObjectTop = 0;
        ObjectWidth = 0;
        ObjectHeight = 0;
        ObjectName = "";
        ObjectText = "";
        ObjectDataBinding = "";
        ObjectExpression = "";
        ObjectFormatString = "";
        ObjectFontName = "Arial";
        ObjectFontSize = 10;
        ObjectBold = false;
        ObjectItalic = false;
        ObjectUnderline = false;
        ObjectForeColor = "#000000";
        ObjectBackColor = "#FFFFFF";
        ObjectAlignment = TextAlignment.Left;
        ObjectWordWrap = true;
        ObjectBorderLeft = false;
        ObjectBorderRight = false;
        ObjectBorderTop = false;
        ObjectBorderBottom = false;
        ObjectBorderWidth = 1;
        ObjectBorderColor = "#000000";
        ObjectCanGrow = true;
        ObjectCanShrink = false;
        ObjectVisible = true;
    }

    private void UpdateObjectProperty(Action<ReportObject> update)
    {
        if (SelectedObject != null)
        {
            update(SelectedObject);
        }
    }

    private void UpdateTextProperty(Action<TextProperties> update)
    {
        if (SelectedObject?.TextProps != null)
        {
            update(SelectedObject.TextProps);
        }
    }

    private void UpdateBorderProperty(Action<BorderProperties> update)
    {
        if (SelectedObject?.Border != null)
        {
            update(SelectedObject.Border);
        }
        else if (SelectedObject != null)
        {
            SelectedObject.Border = new BorderProperties();
            update(SelectedObject.Border);
        }
    }

    [RelayCommand]
    private void ApplyFormat(string format)
    {
        ObjectFormatString = format;
    }

    [RelayCommand]
    private void ResetProperties()
    {
        if (SelectedObject != null)
        {
            LoadObjectProperties(SelectedObject);
        }
    }
}

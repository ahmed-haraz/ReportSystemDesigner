using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReportDesigner.Core.Models;

public class ReportObject : INotifyPropertyChanged
{
    private ObjectType _type = ObjectType.Text;
    private string _name = string.Empty;
    private float _left;
    private float _top;
    private float _width = 100;
    private float _height = 20;
    private string _dataBinding = string.Empty;
    private string _expression = string.Empty;
    private string _text = string.Empty;
    private string _formatString = string.Empty;
    private bool _canGrow = true;
    private bool _canShrink;
    private bool _visible = true;
    private int _zIndex;
    private bool _isSelected;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public ObjectType Type { get => _type; set => SetProperty(ref _type, value); }
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public float Left { get => _left; set => SetProperty(ref _left, value); }
    public float Top { get => _top; set => SetProperty(ref _top, value); }
    public float Width { get => _width; set => SetProperty(ref _width, value); }
    public float Height { get => _height; set => SetProperty(ref _height, value); }
    public string DataBinding { get => _dataBinding; set => SetProperty(ref _dataBinding, value); }
    public string Expression { get => _expression; set => SetProperty(ref _expression, value); }
    public string Text { get => _text; set => SetProperty(ref _text, value); }
    public TextProperties TextProps { get; set; } = new();
    public BorderProperties Border { get; set; } = new();
    public FillProperties Fill { get; set; } = new();
    public string FormatString { get => _formatString; set => SetProperty(ref _formatString, value); }
    public bool CanGrow { get => _canGrow; set => SetProperty(ref _canGrow, value); }
    public bool CanShrink { get => _canShrink; set => SetProperty(ref _canShrink, value); }
    public bool Visible { get => _visible; set => SetProperty(ref _visible, value); }
    public string VisibleExpression { get; set; } = string.Empty;
    public string Hyperlink { get; set; } = string.Empty;
    public string Bookmark { get; set; } = string.Empty;
    public string PrintOn { get; set; } = "AllPages";
    public int ZIndex { get => _zIndex; set => SetProperty(ref _zIndex, value); }
    public string Cursor { get; set; } = "Default";
    public string Tag { get; set; } = string.Empty;
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    public PictureProperties PictureProps { get; set; }
    public BarcodeProperties BarcodeProps { get; set; }
    public ChartProperties ChartProps { get; set; }
    public TableProperties TableProps { get; set; }
    public ShapeProperties ShapeProps { get; set; }
    public SubreportProperties SubreportProps { get; set; }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}

public class PictureProperties
{
    public string ImageSource { get; set; } = string.Empty; 
    public string ImageFormat { get; set; } = "png"; 
    public bool KeepAspectRatio { get; set; } = true;
    public string Alignment { get; set; } = "Center";
    public int Rotation { get; set; } = 0;
    public string GrayscaleExpression { get; set; } = string.Empty;
    public string TransparencyExpression { get; set; } = string.Empty;
}

public class BarcodeProperties
{
    public BarcodeType BarcodeType { get; set; } = BarcodeType.Code128;
    public string Data { get; set; } = string.Empty;
    public bool ShowText { get; set; } = true;
    public float BarWidth { get; set; } = 2;
    public float BarHeight { get; set; } = 50;
    public string TextPosition { get; set; } = "Bottom";
    public bool Checksum { get; set; } = true;
    public string ErrorCorrection { get; set; } = "M"; 
}

public class ChartProperties
{
    public ChartType ChartType { get; set; } = ChartType.Bar;
    public string Title { get; set; } = string.Empty;
    public string XAxisField { get; set; } = string.Empty;
    public string YAxisField { get; set; } = string.Empty;
    public string SeriesField { get; set; } = string.Empty;
    public string ValueField { get; set; } = string.Empty;
    public bool ShowLegend { get; set; } = true;
    public bool ShowValues { get; set; } = true;
    public string ColorPalette { get; set; } = "Default";
    public int Width { get; set; } = 400;
    public int Height { get; set; } = 300;
}

public class TableProperties
{
    public int RowCount { get; set; } = 1;
    public int ColumnCount { get; set; } = 1;
    public List<TableRow> Rows { get; set; } = new();
    public List<TableColumn> Columns { get; set; } = new();
    public bool RepeatHeaderRow { get; set; } = true;
    public string DataSourceName { get; set; } = string.Empty;
    public bool AutoWidth { get; set; } = true;
}

public class TableRow
{
    public float Height { get; set; } = 20;
    public bool IsHeader { get; set; }
    public bool IsFooter { get; set; }
    public List<TableCell> Cells { get; set; } = new();
}

public class TableColumn
{
    public float Width { get; set; } = 100;
    public bool AutoSize { get; set; } = true;
    public string FieldName { get; set; } = string.Empty;
}

public class TableCell : ReportObject
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public int RowSpan { get; set; } = 1;
    public int ColumnSpan { get; set; } = 1;
    public string Aggregate { get; set; } = string.Empty; // Sum, Count, etc.
}

public class ShapeProperties
{
    public ShapeType ShapeType { get; set; } = ShapeType.Rectangle;
    public int CornerRadius { get; set; } = 0;
    public string StartArrow { get; set; } = "None";
    public string EndArrow { get; set; } = "None";
    public int ArrowSize { get; set; } = 5;
    public bool FillShape { get; set; } = true;
}

public class SubreportProperties
{
    public string ReportFile { get; set; } = string.Empty;
    public string ParameterExpression { get; set; } = string.Empty;
    public bool PrintOnParent { get; set; } = true;
    public bool KeepTogether { get; set; } = true;
}

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

    public void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
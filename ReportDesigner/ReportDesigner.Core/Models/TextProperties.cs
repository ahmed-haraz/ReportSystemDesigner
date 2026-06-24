using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReportDesigner.Core.Models;

public class TextProperties : INotifyPropertyChanged
{
    private string _fontName = "Arial";
    private float _fontSize = 10;
    private bool _bold;
    private bool _italic;
    private bool _underline;
    private string _foreColor = "#000000";
    private string _backColor = "#FFFFFF";
    private TextAlignment _alignment = TextAlignment.Left;
    private bool _wordWrap = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string FontName { get => _fontName; set => SetProperty(ref _fontName, value); }
    public float FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
    public bool Bold { get => _bold; set => SetProperty(ref _bold, value); }
    public bool Italic { get => _italic; set => SetProperty(ref _italic, value); }
    public bool Underline { get => _underline; set => SetProperty(ref _underline, value); }
    public bool Strikeout { get; set; }
    public string ForeColor { get => _foreColor; set => SetProperty(ref _foreColor, value); }
    public string BackColor { get => _backColor; set => SetProperty(ref _backColor, value); }
    public TextAlignment Alignment { get => _alignment; set => SetProperty(ref _alignment, value); }
    public bool WordWrap { get => _wordWrap; set => SetProperty(ref _wordWrap, value); }
    public bool AutoShrink { get; set; }
    public bool AutoExpand { get; set; } = true;
    public int Angle { get; set; } = 0;
    public string FormatString { get; set; } = string.Empty;
    public string NullValue { get; set; } = string.Empty;
    public bool RightToLeft { get; set; }
    public float CharSpacing { get; set; } = 0;
    public float LineSpacing { get; set; } = 1;
    public bool TrimWhitespace { get; set; } = true;
    public bool HtmlTags { get; set; }

    private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
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

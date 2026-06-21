namespace ReportDesigner.Core.Models;

public class TextProperties
{
    public string FontName { get; set; } = "Arial";
    public float FontSize { get; set; } = 10;
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Underline { get; set; }
    public bool Strikeout { get; set; }
    public string ForeColor { get; set; } = "#000000";
    public string BackColor { get; set; } = "#FFFFFF";
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    public bool WordWrap { get; set; } = true;
    public bool AutoShrink { get; set; }
    public bool AutoExpand { get; set; } = true;
    public int Angle { get; set; } = 0;
    public string FormatString { get; set; } = string.Empty; // {0:C2}, {0:yyyy-MM-dd}
    public string NullValue { get; set; } = string.Empty;
    public bool RightToLeft { get; set; }
    public float CharSpacing { get; set; } = 0;
    public float LineSpacing { get; set; } = 1;
    public bool TrimWhitespace { get; set; } = true;
    public bool HtmlTags { get; set; } // Allow HTML formatting
}

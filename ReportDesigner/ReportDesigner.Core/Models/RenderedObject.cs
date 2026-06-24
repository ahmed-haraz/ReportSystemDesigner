namespace ReportDesigner.Core.Models;

public class RenderedObject
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public string Text { get; set; } = "";
    public string FontName { get; set; } = "Arial";
    public float FontSize { get; set; } = 10;
    public bool Bold { get; set; }
    public string DataBinding { get; set; } = "";
    public string ObjectType { get; set; } = "Text";
    public bool IsBandBackground { get; set; }
    public string BandName { get; set; } = "";
    public List<RenderedTableCell> TableCells { get; set; } = new();
}


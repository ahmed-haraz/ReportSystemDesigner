namespace ReportDesigner.Core.Models;

public class RenderedTableCell
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public string Text { get; set; } = "";
    public bool IsHeader { get; set; }
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
}
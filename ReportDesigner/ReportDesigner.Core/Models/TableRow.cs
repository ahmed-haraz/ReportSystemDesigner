namespace ReportDesigner.Core.Models;

public class TableRow
{
    public float Height { get; set; } = 20;
    public bool IsHeader { get; set; }
    public bool IsFooter { get; set; }
    public List<TableCell> Cells { get; set; } = new();
}
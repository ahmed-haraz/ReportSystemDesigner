namespace ReportDesigner.Core.Models;

public class TableCell : ReportObject
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public int RowSpan { get; set; } = 1;
    public int ColumnSpan { get; set; } = 1;
    public string Aggregate { get; set; } = string.Empty;

    // NEW: Enhanced data binding for cells
    public string ColumnName { get; set; } = string.Empty;
    public string DataField { get; set; } = string.Empty;
    public bool IsHeaderCell { get; set; }
    public string HeaderText { get; set; } = string.Empty;
}

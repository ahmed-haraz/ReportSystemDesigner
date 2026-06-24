namespace ReportDesigner.Core.Models;

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
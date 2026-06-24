namespace ReportDesigner.Core.Models;

public class TableColumn
{
    public float Width { get; set; } = 100;
    public bool AutoSize { get; set; } = true;
    public string FieldName { get; set; } = string.Empty;
}
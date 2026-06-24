namespace ReportDesigner.Core.Models;

public class SubreportProperties
{
    public string ReportFile { get; set; } = string.Empty;
    public string ParameterExpression { get; set; } = string.Empty;
    public bool PrintOnParent { get; set; } = true;
    public bool KeepTogether { get; set; } = true;
}
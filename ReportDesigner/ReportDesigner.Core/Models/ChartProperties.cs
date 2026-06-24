namespace ReportDesigner.Core.Models;

public class ChartProperties
{
    public ChartType ChartType { get; set; } = ChartType.Bar;
    public string Title { get; set; } = string.Empty;
    public string XAxisField { get; set; } = string.Empty;
    public string YAxisField { get; set; } = string.Empty;
    public string SeriesField { get; set; } = string.Empty;
    public string ValueField { get; set; } = string.Empty;
    public bool ShowLegend { get; set; } = true;
    public bool ShowValues { get; set; } = true;
    public string ColorPalette { get; set; } = "Default";
    public int Width { get; set; } = 400;
    public int Height { get; set; } = 300;
}
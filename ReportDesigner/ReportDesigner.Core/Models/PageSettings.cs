namespace ReportDesigner.Core.Models;

public class PageSettings
{
    public float Width { get; set; } = 595; // A4 in points (72 DPI)
    public float Height { get; set; } = 842;
    public float LeftMargin { get; set; } = 40;
    public float RightMargin { get; set; } = 40;
    public float TopMargin { get; set; } = 40;
    public float BottomMargin { get; set; } = 40;
    public PageOrientation Orientation { get; set; } = PageOrientation.Portrait;
    public float ColumnWidth { get; set; } = 0;
    public int ColumnCount { get; set; } = 1;
    public float ColumnGap { get; set; } = 0;
    public string PaperName { get; set; } = "A4";
    public int Resolution { get; set; } = 96; // DPI

    // Watermark
    public string WatermarkText { get; set; } = string.Empty;
    public string WatermarkFont { get; set; } = "Arial";
    public float WatermarkFontSize { get; set; } = 48;
    public string WatermarkColor { get; set; } = "#C0C0C0";
    public float WatermarkOpacity { get; set; } = 0.3f;
    public bool WatermarkShowBehind { get; set; } = true;
}

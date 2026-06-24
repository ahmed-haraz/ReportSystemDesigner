namespace ReportDesigner.Core.Models;

public class BarcodeProperties
{
    public BarcodeType BarcodeType { get; set; } = BarcodeType.Code128;
    public string Data { get; set; } = string.Empty;
    public bool ShowText { get; set; } = true;
    public float BarWidth { get; set; } = 2;
    public float BarHeight { get; set; } = 50;
    public string TextPosition { get; set; } = "Bottom";
    public bool Checksum { get; set; } = true;
    public string ErrorCorrection { get; set; } = "M";
}

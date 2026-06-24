namespace ReportDesigner.Core.Models;

public class PictureProperties
{
    public string ImageSource { get; set; } = string.Empty;
    public string ImageFormat { get; set; } = "png";
    public bool KeepAspectRatio { get; set; } = true;
    public string Alignment { get; set; } = "Center";
    public int Rotation { get; set; } = 0;
    public string GrayscaleExpression { get; set; } = string.Empty;
    public string TransparencyExpression { get; set; } = string.Empty;
}
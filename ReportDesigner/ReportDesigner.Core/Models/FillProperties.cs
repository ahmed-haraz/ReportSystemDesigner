namespace ReportDesigner.Core.Models;

public class FillProperties
{
    public string Color { get; set; } = "#FFFFFF";
    public FillType Type { get; set; } = FillType.Solid;

    // Gradient properties
    public string GradientStartColor { get; set; } = "#FFFFFF";
    public string GradientEndColor { get; set; } = "#000000";
    public float GradientAngle { get; set; } = 0;

    // Pattern properties
    public string PatternColor { get; set; } = "#000000";
    public string PatternStyle { get; set; } = "Horizontal";

    // Image fill
    public string ImageSource { get; set; } = string.Empty;
    public bool ImageTile { get; set; }
    public string ImageAlignment { get; set; } = "Stretch";
}

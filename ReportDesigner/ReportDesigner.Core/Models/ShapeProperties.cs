namespace ReportDesigner.Core.Models;

public class ShapeProperties
{
    public ShapeType ShapeType { get; set; } = ShapeType.Rectangle;
    public int CornerRadius { get; set; } = 0;
    public string StartArrow { get; set; } = "None";
    public string EndArrow { get; set; } = "None";
    public int ArrowSize { get; set; } = 5;
    public bool FillShape { get; set; } = true;
}

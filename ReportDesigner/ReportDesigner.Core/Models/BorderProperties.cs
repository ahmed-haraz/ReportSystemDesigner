namespace ReportDesigner.Core.Models;

public class BorderProperties
{
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Top { get; set; }
    public bool Bottom { get; set; }
    public float Width { get; set; } = 1;
    public string Color { get; set; } = "#000000";
    public BorderStyle Style { get; set; } = BorderStyle.Solid;
    public float Radius { get; set; } = 0; // For rounded corners

    // Individual border colors (optional)
    public string LeftColor { get; set; }
    public string RightColor { get; set; }
    public string TopColor { get; set; }
    public string BottomColor { get; set; }

    public bool AnyBorder => Left || Right || Top || Bottom;
}

namespace ReportDesigner.Core.Models;

public class ToolboxItem
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Icon { get; set; } = "";
    public bool IsBand { get; set; }
    public string Description { get; set; } = "";
    public bool CanHide { get; set; } // NEW
    public bool CanRemove { get; set; } // NEW
    public bool IsVisible { get; set; } = true; // NEW

    public ObjectType ObjectType => Enum.Parse<ObjectType>(Type);
    public BandType BandType => Enum.Parse<BandType>(Type);
}

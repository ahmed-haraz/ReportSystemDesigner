namespace ReportDesigner.Core.Models;

public sealed record PageSizePreset(
    string Name,
    float Width,
    float Height,
    string Category,
    float DefaultMargin = 40)
{
    public static readonly IReadOnlyList<PageSizePreset> All = new List<PageSizePreset>
    {
        new("A0", 2384, 3370, "ISO A"),
        new("A1", 1684, 2384, "ISO A"),
        new("A2", 1191, 1684, "ISO A"),
        new("A3", 842, 1191, "ISO A"),
        new("A4", 595, 842, "ISO A"),
        new("A5", 420, 595, "ISO A"),
        new("A6", 298, 420, "ISO A"),
        new("A7", 210, 298, "ISO A", 20),
        new("Letter", 612, 792, "US"),
        new("Legal", 612, 1008, "US"),
        new("Executive", 522, 756, "US"),
        new("Receipt 58mm", 164, 842, "Receipt", 8),
        new("Receipt 80mm", 227, 842, "Receipt", 8),
        new("Thermal 3in", 216, 842, "Receipt", 8),
        new("Label 4x6", 288, 432, "Label", 12),
        new("Custom", 595, 842, "Custom")
    };

    public static PageSizePreset Find(string name) =>
        All.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
        ?? All.First(p => p.Name == "A4");
}

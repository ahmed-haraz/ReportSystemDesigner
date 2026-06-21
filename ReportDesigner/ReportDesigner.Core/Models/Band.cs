namespace ReportDesigner.Core.Models;

public class Band
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public BandType Type { get; set; } = BandType.Data;
    public string Name { get; set; } = string.Empty;
    public float Height { get; set; } = 37.8f; // Default ~13.3mm
    public float Top { get; set; }
    public bool CanGrow { get; set; } = true;
    public bool CanShrink { get; set; }
    public bool RepeatOnEveryPage { get; set; }
    public bool PrintOnBottom { get; set; } // For page footer
    public bool StartNewPage { get; set; } // Force page break before
    public bool PrintIfDetailEmpty { get; set; } = true;
    public string Condition { get; set; } = string.Empty; // PrintIf expression
    public string BeforePrintScript { get; set; } = string.Empty;
    public string AfterPrintScript { get; set; } = string.Empty;
    public string DataSourceName { get; set; } = string.Empty; // For Data bands
    public string FilterExpression { get; set; } = string.Empty; // Row filter
    public string SortExpression { get; set; } = string.Empty; // Sort order
    public string GroupExpression { get; set; } = string.Empty; // For group bands
    public bool KeepTogether { get; set; } = true; // Prevent band splitting across pages
    public bool Outline { get; set; } // Include in document outline
    public string OutlineText { get; set; } = string.Empty;
    public int OutlineLevel { get; set; } = 1;
    public List<ReportObject> Objects { get; set; } = new();
    public List<Band> ChildBands { get; set; } = new(); // Nested bands

    // Layout helpers (not serialized)
    public bool IsSelected { get; set; }
    public bool IsExpanded { get; set; } = true;
    public string DisplayName => $"{Type}Band_{Name}";
}

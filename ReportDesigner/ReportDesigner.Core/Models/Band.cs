namespace ReportDesigner.Core.Models;

public class Band
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public BandType Type { get; set; } = BandType.Data;
    public string Name { get; set; } = string.Empty;
    public float Height { get; set; } = 37.8f;
    public float Top { get; set; }
    public bool CanGrow { get; set; } = true;
    public bool CanShrink { get; set; }
    public bool RepeatOnEveryPage { get; set; }
    public bool PrintOnBottom { get; set; }
    public bool StartNewPage { get; set; }
    public bool PrintIfDetailEmpty { get; set; } = true;
    public string Condition { get; set; } = string.Empty;
    public string BeforePrintScript { get; set; } = string.Empty;
    public string AfterPrintScript { get; set; } = string.Empty;
    public string DataSourceName { get; set; } = string.Empty;
    public string FilterExpression { get; set; } = string.Empty;
    public string SortExpression { get; set; } = string.Empty;
    public string GroupExpression { get; set; } = string.Empty;
    public bool KeepTogether { get; set; } = true;
    public bool Outline { get; set; }
    public string OutlineText { get; set; } = string.Empty;
    public int OutlineLevel { get; set; } = 1;
    public List<ReportObject> Objects { get; set; } = new();
    public List<Band> ChildBands { get; set; } = new();

    // NEW: Band visibility
    public bool Visible { get; set; } = true;
    public string VisibleExpression { get; set; } = string.Empty;

    // Layout helpers
    public bool IsSelected { get; set; }
    public bool IsExpanded { get; set; } = true;
    public bool IsVisibleInDesigner { get; set; } = true; // NEW: Hide in designer
    public string DisplayName => $"{Type}Band_{Name}";
}

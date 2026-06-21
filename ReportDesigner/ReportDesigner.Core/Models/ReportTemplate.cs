namespace ReportDesigner.Core.Models;

public class ReportTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Untitled Report";
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
    public string Version { get; set; } = "1.0";
    public string BaseReport { get; set; } = string.Empty; // Inheritance
    public PageSettings Page { get; set; } = new();
    public List<Band> Bands { get; set; } = new();
    public List<DataSource> DataSources { get; set; } = new();
    public List<ReportParameter> Parameters { get; set; } = new();
    public Dictionary<string, string> Styles { get; set; } = new();
    public string Script { get; set; } = string.Empty; // C# script for complex logic
    public string ScriptLanguage { get; set; } = "CSharp"; // CSharp, VB.NET, JavaScript
    public bool ConvertNulls { get; set; } = true;
    public string NullValue { get; set; } = string.Empty;
    public bool DoublePass { get; set; } // Two-pass report for aggregates
    public string ReportInfo { get; set; } = string.Empty; // Metadata
    public bool AutoFillDataSet { get; set; } = true;
    public string DataSetName { get; set; } = string.Empty;
    public int MaxRows { get; set; } = 0; // 0 = unlimited
    public bool Compressed { get; set; } // Compress template file
    public string EncryptionKey { get; set; } = string.Empty; // Encrypt sensitive data
    public List<string> ReferencedAssemblies { get; set; } = new(); // Custom assemblies
    public Dictionary<string, object> UserData { get; set; } = new(); // Custom properties

    // Helper properties
    public float ContentWidth => Page.Width - Page.LeftMargin - Page.RightMargin;
    public float ContentHeight => Page.Height - Page.TopMargin - Page.BottomMargin;

    public Band? GetBand(BandType type) => Bands.FirstOrDefault(b => b.Type == type);
    public IEnumerable<Band> GetBands(BandType type) => Bands.Where(b => b.Type == type);
    public void AddBand(Band band) => Bands.Add(band);
    public void RemoveBand(string id) => Bands.RemoveAll(b => b.Id == id);
    public void MoveBand(string id, int newIndex)
    {
        var band = Bands.FirstOrDefault(b => b.Id == id);
        if (band != null)
        {
            Bands.Remove(band);
            Bands.Insert(Math.Clamp(newIndex, 0, Bands.Count), band);
        }
    }
}

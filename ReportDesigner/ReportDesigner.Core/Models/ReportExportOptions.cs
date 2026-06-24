namespace ReportDesigner.Core.Models;

public class ReportExportOptions
{
    public string TargetFramework { get; set; } = "net10.0";
    public string MauiResourcePath { get; set; } = "Resources/Raw";
    public bool IncludeSampleDataContract { get; set; } = true;
    public bool IncludeConnectionMetadata { get; set; } = true;
    public bool EmbedTemplateJson { get; set; } = true;
    public Dictionary<string, string> RuntimeHints { get; set; } = new()
    {
        ["renderer"] = "MauiReportEngine.Renderer",
        ["storage"] = "BundleResource or app data file",
        ["output"] = "PDF/print/share from MAUI services"
    };
}

public class MauiReportPackage
{
    public string PackageVersion { get; set; } = "1.0";
    public string TemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string TargetFramework { get; set; } = "net10.0";
    public string MauiResourcePath { get; set; } = "Resources/Raw";
    public string TemplateJson { get; set; } = string.Empty;
    public List<DataSourceManifest> DataSources { get; set; } = new();
    public List<string> RequiredNuGetPackages { get; set; } = new();
    public Dictionary<string, string> RuntimeHints { get; set; } = new();
}

public class DataSourceManifest
{
    public string Name { get; set; } = string.Empty;
    public DataSourceType Type { get; set; }
    public string SelectCommand { get; set; } = string.Empty;
    public List<DataColumn> Columns { get; set; } = new();
    public bool RequiresRuntimeConnectionString { get; set; } = true;
}

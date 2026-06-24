using System.Text.Json;
using ReportDesigner.Core.Models;

namespace ReportDesigner.Core.Export;

public static class MauiReportPackageExporter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public static MauiReportPackage CreatePackage(ReportTemplate template, ReportExportOptions? options = null)
    {
        options ??= new ReportExportOptions();

        return new MauiReportPackage
        {
            TemplateId = template.Id,
            TemplateName = template.Name,
            TargetFramework = options.TargetFramework,
            MauiResourcePath = options.MauiResourcePath,
            TemplateJson = options.EmbedTemplateJson ? JsonSerializer.Serialize(template, JsonOptions) : string.Empty,
            DataSources = template.DataSources.Select(ds => new DataSourceManifest
            {
                Name = ds.Name,
                Type = ds.Type,
                SelectCommand = ds.SelectCommand,
                Columns = ds.Columns.ToList(),
                RequiresRuntimeConnectionString = !string.IsNullOrWhiteSpace(ds.ConnectionString)
            }).ToList(),
            RequiredNuGetPackages = GetRequiredPackages(template.DataSources).ToList(),
            RuntimeHints = new Dictionary<string, string>(options.RuntimeHints)
        };
    }

    public static string ExportPackageJson(ReportTemplate template, ReportExportOptions? options = null) =>
        JsonSerializer.Serialize(CreatePackage(template, options), JsonOptions);

    public static async Task SavePackageAsync(ReportTemplate template, string filePath, ReportExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        var json = ExportPackageJson(template, options);
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    private static IEnumerable<string> GetRequiredPackages(IEnumerable<DataSource> dataSources)
    {
        yield return "MauiReportEngine.Core";
        yield return "MauiReportEngine.Renderer";

        foreach (var type in dataSources.Select(ds => ds.Type).Distinct())
        {
            switch (type)
            {
                case DataSourceType.SQLite:
                    yield return "Microsoft.Data.Sqlite";
                    break;
                case DataSourceType.SqlServer:
                    yield return "Microsoft.Data.SqlClient";
                    break;
                case DataSourceType.PostgreSQL:
                    yield return "Npgsql";
                    break;
                case DataSourceType.MySQL:
                    yield return "MySqlConnector";
                    break;
            }
        }
    }
}

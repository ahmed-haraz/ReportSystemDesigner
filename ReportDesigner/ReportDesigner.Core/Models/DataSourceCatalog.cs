namespace ReportDesigner.Core.Models;

public sealed record DataSourceDefinition(
    DataSourceType Type,
    string DisplayName,
    string Category,
    string ConnectionStringExample,
    string NuGetPackage,
    bool SupportsSchemaDiscovery,
    bool RequiresClientPackage = true);

public static class DataSourceCatalog
{
    public static readonly IReadOnlyList<DataSourceDefinition> All = new List<DataSourceDefinition>
    {
        new(DataSourceType.SQLite, "SQLite", "Database", "Data Source=app.db", "Microsoft.Data.Sqlite", true),
        new(DataSourceType.SqlServer, "SQL Server", "Database", "Server=.;Database=AppDb;Trusted_Connection=True;TrustServerCertificate=True", "Microsoft.Data.SqlClient", true),
        new(DataSourceType.PostgreSQL, "PostgreSQL", "Database", "Host=localhost;Database=app;Username=postgres;Password=***", "Npgsql", true),
        new(DataSourceType.MySQL, "MySQL/MariaDB", "Database", "Server=localhost;Database=app;User ID=root;Password=***", "MySqlConnector", true),
        new(DataSourceType.Oracle, "Oracle", "Database", "User Id=user;Password=***;Data Source=host/service", "Oracle.ManagedDataAccess.Core", false),
        new(DataSourceType.Json, "JSON", "File/API", "file://data.json or https://api.example.com/data", "System.Text.Json", false, false),
        new(DataSourceType.Xml, "XML", "File/API", "file://data.xml", "System.Xml.ReaderWriter", false, false),
        new(DataSourceType.Csv, "CSV", "File", "file://data.csv", "CsvHelper", false),
        new(DataSourceType.Excel, "Excel", "File", "file://workbook.xlsx", "ExcelDataReader", false),
        new(DataSourceType.OData, "OData", "API", "https://services.odata.org/...", "Microsoft.OData.Client", false),
        new(DataSourceType.WebService, "REST/Web service", "API", "https://api.example.com/report", "System.Net.Http.Json", false, false),
        new(DataSourceType.BusinessObject, "Business Object", "Runtime", "Assembly=MyApp;Type=MyApp.ReportData", "", false, false),
        new(DataSourceType.MongoDB, "MongoDB", "NoSQL", "mongodb://localhost:27017/app", "MongoDB.Driver", false)
    };

    public static DataSourceDefinition Get(DataSourceType type) => All.First(d => d.Type == type);
}

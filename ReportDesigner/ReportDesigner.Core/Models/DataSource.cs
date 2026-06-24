namespace ReportDesigner.Core.Models;

public class DataSource
{
    public string Name { get; set; } = string.Empty;
    public DataSourceType Type { get; set; } = DataSourceType.Json;
    public string ConnectionString { get; set; } = string.Empty;
    public string SelectCommand { get; set; } = string.Empty;
    public List<DataColumn> Columns { get; set; } = new();
    public List<ReportParameter> Parameters { get; set; } = new();
    public string Schema { get; set; } = string.Empty; // JSON schema or XML schema
    public bool Enabled { get; set; } = true;
    public int CommandTimeout { get; set; } = 30;
    public string TableName { get; set; } = string.Empty; // For business objects
    public string AssemblyName { get; set; } = string.Empty; // For business objects
    public string ClassName { get; set; } = string.Empty; // For business objects
    public string MethodName { get; set; } = string.Empty; // For business objects
    public Dictionary<string, string> ProviderOptions { get; set; } = new();
    public string RuntimeProviderKey { get; set; } = string.Empty;

    public DataSourceDefinition Definition => DataSourceCatalog.Get(Type);
}

public class DataColumn
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = "System.String";
    public string Caption { get; set; } = string.Empty;
    public int Size { get; set; }
    public bool Nullable { get; set; } = true;
    public bool PrimaryKey { get; set; }
    public bool ReadOnly { get; set; }
    public string Expression { get; set; } = string.Empty; // Calculated column
    public string Format { get; set; } = string.Empty;
}

using ReportDesigner.Core.Models;
using System.Data;
using DataColumn = ReportDesigner.Core.Models.DataColumn;

namespace ReportDesigner.Core.Interfaces;

public interface IDataConnection
{
    string Name { get; set; }
    DataSourceType Type { get; }
    string ConnectionString { get; set; }
    bool IsConnected { get; }

    Task<bool> ConnectAsync();
    Task DisconnectAsync();
    Task<List<string>> GetTablesAsync();
    Task<List<DataColumn>> GetColumnsAsync(string tableName);
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters);
    Task<List<string>> GetDatabasesAsync();
    Task<bool> TestConnectionAsync();
    Task<DataTable> GetSchemaAsync();
    string BuildConnectionString(Dictionary<string, string> parameters);
}

public interface IConnectionFactory
{
    IDataConnection CreateConnection(DataSourceType type);
    List<DataSourceType> SupportedTypes { get; }
}

using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using ReportDesigner.Core.Models;
using System.IO;
using DataColumn = ReportDesigner.Core.Models.DataColumn;

namespace ReportDesigner.Desktop.Services;


public class ConnectionWizard
{
    public async Task<DataSource> CreateSQLiteConnectionAsync(string dbPath)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWrite
        };

        var dataSource = new DataSource
        {
            Name = $"SQLite_{Path.GetFileNameWithoutExtension(dbPath)}",
            Type = DataSourceType.SQLite,
            ConnectionString = builder.ConnectionString
        };

        using var connection = new SqliteConnection(builder.ConnectionString);
        await connection.OpenAsync();

        var tables = connection.GetSchema("Tables");
        foreach (DataRow row in tables.Rows)
        {
            var tableName = row["TABLE_NAME"].ToString();
            dataSource.Columns.Add(new DataColumn
            {
                Name = tableName!,
                DataType = "Table"
            });
        }

        return dataSource;
    }

    public async Task<DataSource> CreateSqlServerConnectionAsync(string server, string database, string username, string password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = username,
            Password = password,
            TrustServerCertificate = true
        };

        var dataSource = new DataSource
        {
            Name = $"SQL_{database}",
            Type = DataSourceType.SqlServer,
            ConnectionString = builder.ConnectionString
        };

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        var tables = connection.GetSchema("Tables");
        foreach (DataRow row in tables.Rows)
        {
            var tableName = row["TABLE_NAME"].ToString();
            dataSource.Columns.Add(new DataColumn
            {
                Name = tableName!,
                DataType = "Table"
            });
        }

        return dataSource;
    }

    public async Task<DataTable> PreviewDataAsync(DataSource dataSource, string query, Dictionary<string, object>? parameters = null)
    {
        switch (dataSource.Type)
        {
            case DataSourceType.SQLite:
                return await ExecuteSQLiteQueryAsync(dataSource.ConnectionString, query, parameters);
            case DataSourceType.SqlServer:
                return await ExecuteSqlServerQueryAsync(dataSource.ConnectionString, query, parameters);
            default:
                throw new NotSupportedException($"Data source type {dataSource.Type} not supported");
        }
    }

    private async Task<DataTable> ExecuteSQLiteQueryAsync(string connectionString, string query, Dictionary<string, object>? parameters)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqliteCommand(query, connection);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        var dataTable = new DataTable();
        using var reader = await command.ExecuteReaderAsync();
        dataTable.Load(reader);

        return dataTable;
    }

    private async Task<DataTable> ExecuteSqlServerQueryAsync(string connectionString, string query, Dictionary<string, object>? parameters)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        var dataTable = new DataTable();
        using var reader = await command.ExecuteReaderAsync();
        dataTable.Load(reader);

        return dataTable;
    }
}

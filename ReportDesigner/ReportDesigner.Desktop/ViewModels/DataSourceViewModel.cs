using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using System.Windows;
using DataColumn = ReportDesigner.Core.Models.DataColumn;

namespace ReportDesigner.Desktop.ViewModels;

public partial class DataSourceViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<DataSource> _dataSources = new();

    [ObservableProperty]
    private DataSource? _selectedDataSource;

    [ObservableProperty]
    private string _connectionString = "";

    [ObservableProperty]
    private string _selectCommand = "";

    [ObservableProperty]
    private ObservableCollection<DataColumn> _columns = new();

    [ObservableProperty]
    private ObservableCollection<string> _tables = new();

    [ObservableProperty]
    private string _selectedTable = "";

    [ObservableProperty]
    private ObservableCollection<DataSourceDefinition> _availableDataSources = new(DataSourceCatalog.All);

    [ObservableProperty]
    private string _statusMessage = "";

    partial void OnSelectedDataSourceChanged(DataSource? value)
    {
        if (value != null)
        {
            ConnectionString = value.ConnectionString;
            SelectCommand = value.SelectCommand;
            Columns = new ObservableCollection<DataColumn>(value.Columns);
            Tables.Clear();
            SelectedTable = "";
        }
    }

    [RelayCommand]
    private void AddDataSource()
    {
        var dialog = new Views.ConnectionDialog();
        if (dialog.ShowDialog() == true)
        {
            var ds = new DataSource
            {
                Name = $"DataSource{DataSources.Count + 1}",
                Type = Enum.TryParse<DataSourceType>(dialog.SelectedDataSourceType, out var selectedType)
                    ? selectedType : DataSourceType.SQLite,
                ConnectionString = dialog.ConnectionString ?? DataSourceCatalog.Get(DataSourceType.SQLite).ConnectionStringExample,
                SelectCommand = "SELECT * FROM YourTable",
                Enabled = true
            };

            DataSources.Add(ds);
            SelectedDataSource = ds;
            StatusMessage = $"Added data source: {ds.Name}";

            // Auto-discover tables
            _ = DiscoverTablesAsync(ds);
        }
    }

    [RelayCommand]
    private void RemoveDataSource()
    {
        if (SelectedDataSource == null) return;

        var name = SelectedDataSource.Name;
        DataSources.Remove(SelectedDataSource);
        SelectedDataSource = DataSources.FirstOrDefault();

        StatusMessage = $"Removed data source: {name}";
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        if (SelectedDataSource == null)
        {
            MessageBox.Show("Please select a data source first.", "Test Connection", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            bool result = false;

            switch (SelectedDataSource.Type)
            {
                case DataSourceType.SQLite:
                    result = await TestSQLiteConnectionAsync(SelectedDataSource.ConnectionString);
                    break;
                case DataSourceType.SqlServer:
                    result = await TestSqlServerConnectionAsync(SelectedDataSource.ConnectionString);
                    break;
                default:
                    MessageBox.Show($"Direct test is not implemented for {SelectedDataSource.Type}. Add a runtime provider in MAUI using provider key/source options.",
                        "Test Connection", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
            }

            if (result)
            {
                MessageBox.Show("Connection successful!", "Test Connection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                StatusMessage = "Connection test successful";

                // Auto-discover tables after successful connection
                await DiscoverTablesAsync(SelectedDataSource);
            }
            else
            {
                MessageBox.Show("Connection failed.", "Test Connection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusMessage = "Connection test failed";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection error: {ex.Message}", "Test Connection", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage = $"Connection error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RefreshSchema()
    {
        if (SelectedDataSource == null) return;

        await DiscoverTablesAsync(SelectedDataSource);
    }

    [RelayCommand]
    private void SelectTable(string tableName)
    {
        SelectedTable = tableName;

        if (SelectedDataSource != null)
        {
            SelectedDataSource.SelectCommand = $"SELECT * FROM {tableName}";
            SelectCommand = SelectedDataSource.SelectCommand;

            // Discover columns for this table
            _ = DiscoverColumnsAsync(SelectedDataSource, tableName);
        }
    }

    [RelayCommand]
    private void EditConnectionString()
    {
        if (SelectedDataSource == null) return;

        var dialog = new Views.ConnectionDialog();
        if (dialog.ShowDialog() == true)
        {
            SelectedDataSource.ConnectionString = dialog.ConnectionString ?? ConnectionString;
            ConnectionString = SelectedDataSource.ConnectionString;
            OnPropertyChanged(nameof(SelectedDataSource));
        }
    }

    [RelayCommand]
    private void AddParameter()
    {
        if (SelectedDataSource == null) return;

        var param = new ReportParameter
        {
            Name = $"Param{SelectedDataSource.Parameters.Count + 1}",
            DataType = "System.String",
            Description = "New parameter"
        };

        SelectedDataSource.Parameters.Add(param);
        OnPropertyChanged(nameof(SelectedDataSource));
    }

    [RelayCommand]
    private void RemoveParameter(ReportParameter param)
    {
        if (SelectedDataSource == null) return;

        SelectedDataSource.Parameters.Remove(param);
        OnPropertyChanged(nameof(SelectedDataSource));
    }

    [RelayCommand]
    private void BuildSelectCommand()
    {
        if (SelectedDataSource == null || string.IsNullOrEmpty(SelectedTable)) return;

        var columns = string.Join(", ", Columns.Select(c => c.Name));
        if (string.IsNullOrEmpty(columns)) columns = "*";

        SelectedDataSource.SelectCommand = $"SELECT {columns} FROM {SelectedTable}";
        SelectCommand = SelectedDataSource.SelectCommand;
    }

    private async Task DiscoverTablesAsync(DataSource dataSource)
    {
        try
        {
            Tables.Clear();

            switch (dataSource.Type)
            {
                case DataSourceType.SQLite:
                    await DiscoverSQLiteTablesAsync(dataSource);
                    break;
                case DataSourceType.SqlServer:
                    await DiscoverSqlServerTablesAsync(dataSource);
                    break;
                default:
                    StatusMessage = $"Schema discovery for {dataSource.Type} is configured as runtime/manual.";
                    return;
            }

            StatusMessage = $"Discovered {Tables.Count} tables/views";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error discovering tables: {ex.Message}";
        }
    }

    private async Task DiscoverSQLiteTablesAsync(DataSource dataSource)
    {
        using var connection = new SqliteConnection(dataSource.ConnectionString);
        await connection.OpenAsync();

        // Get all tables
        using var command = new SqliteCommand(
            "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name", 
            connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Tables.Add(reader.GetString(0));
        }

        // Get all views
        using var viewCommand = new SqliteCommand(
            "SELECT name FROM sqlite_master WHERE type='view' ORDER BY name", 
            connection);

        using var viewReader = await viewCommand.ExecuteReaderAsync();
        while (await viewReader.ReadAsync())
        {
            Tables.Add(viewReader.GetString(0) + " (view)");
        }
    }

    private async Task DiscoverSqlServerTablesAsync(DataSource dataSource)
    {
        using var connection = new SqlConnection(dataSource.ConnectionString);
        await connection.OpenAsync();

        var tables = connection.GetSchema("Tables");
        foreach (DataRow row in tables.Rows)
        {
            var tableName = row["TABLE_NAME"].ToString();
            var schema = row["TABLE_SCHEMA"].ToString();
            Tables.Add($"{schema}.{tableName}");
        }
    }

    private async Task DiscoverColumnsAsync(DataSource dataSource, string tableName)
    {
        try
        {
            Columns.Clear();

            // Remove (view) suffix if present
            tableName = tableName.Replace(" (view)", "");

            switch (dataSource.Type)
            {
                case DataSourceType.SQLite:
                    await DiscoverSQLiteColumnsAsync(dataSource, tableName);
                    break;
                case DataSourceType.SqlServer:
                    await DiscoverSqlServerColumnsAsync(dataSource, tableName);
                    break;
            }

            dataSource.Columns = Columns.ToList();
            StatusMessage = $"Discovered {Columns.Count} columns in {tableName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error discovering columns: {ex.Message}";
        }
    }

    private async Task DiscoverSQLiteColumnsAsync(DataSource dataSource, string tableName)
    {
        using var connection = new SqliteConnection(dataSource.ConnectionString);
        await connection.OpenAsync();

        // Use PRAGMA table_info
        using var command = new SqliteCommand($"PRAGMA table_info({tableName})", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var colName = reader.GetString(1);
            var dataType = reader.GetString(2);
            var notNull = reader.GetInt32(3) == 1;
            var defaultValue = reader.IsDBNull(4) ? null : reader.GetValue(4)?.ToString();
            var pk = reader.GetInt32(5) == 1;

            Columns.Add(new DataColumn
            {
                Name = colName,
                DataType = MapSQLiteType(dataType),
                Caption = colName,
                Nullable = !notNull,
                PrimaryKey = pk
            });
        }
    }

    private async Task DiscoverSqlServerColumnsAsync(DataSource dataSource, string tableName)
    {
        using var connection = new SqlConnection(dataSource.ConnectionString);
        await connection.OpenAsync();

        // Parse schema.table
        var parts = tableName.Split('.');
        var schema = parts.Length > 1 ? parts[0] : "dbo";
        var name = parts.Length > 1 ? parts[1] : parts[0];

        using var command = new SqlCommand(
            @"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
              FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName
              ORDER BY ORDINAL_POSITION", connection);

        command.Parameters.AddWithValue("@schema", schema);
        command.Parameters.AddWithValue("@tableName", name);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var colName = reader.GetString(0);
            var dataType = reader.GetString(1);
            var isNullable = reader.GetString(2) == "YES";
            var maxLength = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);

            Columns.Add(new DataColumn
            {
                Name = colName,
                DataType = MapSqlServerType(dataType),
                Caption = colName,
                Nullable = isNullable,
                Size = maxLength
            });
        }
    }

    private string MapSQLiteType(string sqliteType)
    {
        var type = sqliteType.ToUpper();
        if (type.Contains("INT")) return "System.Int64";
        if (type.Contains("REAL") || type.Contains("FLOA") || type.Contains("DOUB")) return "System.Double";
        if (type.Contains("NUMERIC") || type.Contains("DECIMAL")) return "System.Decimal";
        if (type.Contains("BOOL")) return "System.Boolean";
        if (type.Contains("DATE") || type.Contains("TIME")) return "System.DateTime";
        if (type.Contains("BLOB")) return "System.Byte[]";
        return "System.String";
    }

    private string MapSqlServerType(string sqlType)
    {
        var type = sqlType.ToUpper();
        return type switch
        {
            "INT" => "System.Int32",
            "BIGINT" => "System.Int64",
            "SMALLINT" => "System.Int16",
            "TINYINT" => "System.Byte",
            "BIT" => "System.Boolean",
            "FLOAT" => "System.Double",
            "REAL" => "System.Single",
            "DECIMAL" => "System.Decimal",
            "NUMERIC" => "System.Decimal",
            "MONEY" => "System.Decimal",
            "SMALLMONEY" => "System.Decimal",
            "DATE" => "System.DateTime",
            "DATETIME" => "System.DateTime",
            "DATETIME2" => "System.DateTime",
            "SMALLDATETIME" => "System.DateTime",
            "TIME" => "System.TimeSpan",
            "DATETIMEOFFSET" => "System.DateTimeOffset",
            "VARBINARY" => "System.Byte[]",
            "BINARY" => "System.Byte[]",
            "IMAGE" => "System.Byte[]",
            "UNIQUEIDENTIFIER" => "System.Guid",
            _ => "System.String"
        };
    }

    private async Task<bool> TestSQLiteConnectionAsync(string connectionString)
    {
        try
        {
            using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> TestSqlServerConnectionAsync(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void AddDataSourceFromDialog(string type, string connectionString, string selectCommand)
    {
        var ds = new DataSource
        {
            Name = $"{type}Source{DataSources.Count + 1}",
            Type = type == "SQLite" ? DataSourceType.SQLite : DataSourceType.SqlServer,
            ConnectionString = connectionString,
            SelectCommand = selectCommand,
            Enabled = true
        };

        DataSources.Add(ds);
        SelectedDataSource = ds;
        StatusMessage = $"Added {type} data source: {ds.Name}";

        _ = DiscoverTablesAsync(ds);
    }
}

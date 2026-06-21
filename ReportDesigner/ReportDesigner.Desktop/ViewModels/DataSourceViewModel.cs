using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;

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

    [RelayCommand]
    private void AddDataSource()
    {
        var ds = new DataSource
        {
            Name = $"DataSource{DataSources.Count + 1}",
            Type = DataSourceType.SQLite
        };
        DataSources.Add(ds);
    }

    [RelayCommand]
    private void RemoveDataSource(DataSource ds)
    {
        DataSources.Remove(ds);
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        if (SelectedDataSource == null) return;
        await Task.Delay(100);
    }

    [RelayCommand]
    private async Task RefreshSchema()
    {
        if (SelectedDataSource == null) return;
        await Task.Delay(100);
    }
}

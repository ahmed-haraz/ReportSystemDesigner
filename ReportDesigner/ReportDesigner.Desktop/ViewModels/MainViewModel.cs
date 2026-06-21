using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using ReportDesigner.Core.Parsers;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace ReportDesigner.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportTemplate _currentReport = new();

    [ObservableProperty]
    private DesignViewModel _designViewModel = new();

    [ObservableProperty]
    private PropertiesViewModel _propertiesViewModel = new();

    [ObservableProperty]
    private DataSourceViewModel _dataSourceViewModel = new();

    [ObservableProperty]
    private ToolboxViewModel _toolboxViewModel = new();

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _selectedObjectInfo = "";

    [ObservableProperty]
    private string _cursorPosition = "0, 0";

    [ObservableProperty]
    private bool _showGrid = true;

    [ObservableProperty]
    private bool _showRulers = true;

    [ObservableProperty]
    private bool _showGuides = true;

    [ObservableProperty]
    private string _selectedZoom = "100%";

    public ObservableCollection<string> ZoomLevels { get; } = new()
    {
        "25%", "50%", "75%", "100%", "125%", "150%", "200%", "400%"
    };

    public ObservableCollection<Band> ReportBands => new(CurrentReport.Bands);

    [RelayCommand]
    private void NewReport()
    {
        CurrentReport = new ReportTemplate { Name = "New Report" };
        StatusMessage = "New report created";
    }

    [RelayCommand]
    private async Task OpenReport()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "FastReport files (*.frx)|*.frx|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            var serializer = new FrxSerializer();
            var content = await File.ReadAllTextAsync(dialog.FileName);
            CurrentReport = serializer.Deserialize(content);
            StatusMessage = $"Opened: {dialog.FileName}";
        }
    }

    [RelayCommand]
    private async Task SaveReport()
    {
        var serializer = new FrxSerializer();
        var content = serializer.Serialize(CurrentReport);

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "FastReport files (*.frx)|*.frx",
            FileName = CurrentReport.Name + ".frx"
        };

        if (dialog.ShowDialog() == true)
        {
            await File.WriteAllTextAsync(dialog.FileName, content);
            StatusMessage = $"Saved: {dialog.FileName}";
        }
    }

    [RelayCommand]
    private async Task SaveAsReport()
    {
        await SaveReport();
    }

    [RelayCommand]
    private void Preview()
    {
        var previewWindow = new Views.PreviewWindow(CurrentReport);
        previewWindow.Show();
    }

    [RelayCommand]
    private void DataSources()
    {
        var dialog = new Views.ConnectionDialog();
        dialog.ShowDialog();
    }

    [RelayCommand]
    private void Parameters()
    {
        MessageBox.Show("Parameters dialog would open here");
    }

    [RelayCommand]
    private void Styles()
    {
        MessageBox.Show("Styles dialog would open here");
    }

    [RelayCommand]
    private void Print()
    {
        MessageBox.Show("Print dialog would open here");
    }

    [RelayCommand]
    private void Undo()
    {
        StatusMessage = "Undo not implemented";
    }

    [RelayCommand]
    private void Redo()
    {
        StatusMessage = "Redo not implemented";
    }

    [RelayCommand]
    private void Cut()
    {
        StatusMessage = "Cut not implemented";
    }

    [RelayCommand]
    private void Copy()
    {
        StatusMessage = "Copy not implemented";
    }

    [RelayCommand]
    private void Paste()
    {
        StatusMessage = "Paste not implemented";
    }

    [RelayCommand]
    private void Delete()
    {
        StatusMessage = "Delete not implemented";
    }

    [RelayCommand]
    private void Documentation()
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://docs.microsoft.com",
            UseShellExecute = true
        });
    }

    [RelayCommand]
    private void About()
    {
        MessageBox.Show("Report Designer v1.0\nBuilt with .NET 10 and WPF", "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }
}

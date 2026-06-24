using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using ReportDesigner.Core.Parsers;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

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

    [ObservableProperty]
    private double _zoomValue = 1.0;

    [ObservableProperty]
    private string _selectedPaperName = "A4";

    [ObservableProperty]
    private PageOrientation _selectedPageOrientation = PageOrientation.Portrait;

    [ObservableProperty]
    private float _pageWidthInput = 595;

    [ObservableProperty]
    private float _pageHeightInput = 842;

    public ObservableCollection<string> PaperSizeOptions { get; } = new(PageSizePreset.All.Select(p => p.Name));

    public ObservableCollection<PageOrientation> PageOrientationOptions { get; } = new(Enum.GetValues<PageOrientation>());

    public ObservableCollection<string> ZoomLevels { get; } = new()
    {
        "25%", "50%", "75%", "100%", "125%", "150%", "200%", "400%"
    };

    public ObservableCollection<Band> ReportBands => new(CurrentReport.Bands);

    public MainViewModel()
    {
        // Wire up child viewmodels to main report
        DesignViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DesignViewModel.Bands))
            {
                CurrentReport.Bands = DesignViewModel.Bands.ToList();
                OnPropertyChanged(nameof(ReportBands));
            }
            else if (e.PropertyName == nameof(DesignViewModel.SelectedObject))
            {
                PropertiesViewModel.SelectedObject = DesignViewModel.SelectedObject;
                SelectedObjectInfo = DesignViewModel.SelectedObject == null
                    ? string.Empty
                    : $"{DesignViewModel.SelectedObject.Name} ({DesignViewModel.SelectedObject.Type})";
            }
            else if (e.PropertyName == nameof(DesignViewModel.SelectedBand))
            {
                PropertiesViewModel.SelectedBand = DesignViewModel.SelectedBand;
            }
        };

        DataSourceViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DataSourceViewModel.DataSources))
            {
                CurrentReport.DataSources = DataSourceViewModel.DataSources.ToList();
            }
        };

        // Initialize with default bands
        InitializeDefaultReport();
    }

    private void InitializeDefaultReport()
    {
        CurrentReport.Name = "New Report";
        CurrentReport.Page = new PageSettings();
        CurrentReport.Page.ApplyPreset(SelectedPaperName, SelectedPageOrientation);
        SyncPageToDesigner();

        // Add default bands
        DesignViewModel.AddBand(BandType.ReportTitle);
        DesignViewModel.AddBand(BandType.PageHeader);
        DesignViewModel.AddBand(BandType.Data);
        DesignViewModel.AddBand(BandType.PageFooter);

        CurrentReport.Bands = DesignViewModel.Bands.ToList();
    }

    partial void OnSelectedZoomChanged(string value)
    {
        if (double.TryParse(value.Replace("%", ""), out var zoom))
        {
            ZoomValue = zoom / 100.0;
            DesignViewModel.Zoom = (float)ZoomValue;
            StatusMessage = $"Zoom set to {value}";
        }
    }

    partial void OnSelectedPaperNameChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        ApplyPageSettingsFromToolbar();
    }

    partial void OnSelectedPageOrientationChanged(PageOrientation value) => ApplyPageSettingsFromToolbar();
    partial void OnPageWidthInputChanged(float value) { if (SelectedPaperName == "Custom") ApplyPageSettingsFromToolbar(); }
    partial void OnPageHeightInputChanged(float value) { if (SelectedPaperName == "Custom") ApplyPageSettingsFromToolbar(); }

    private void ApplyPageSettingsFromToolbar()
    {
        if (CurrentReport.Page == null)
        {
            CurrentReport.Page = new PageSettings();
        }

        if (SelectedPaperName == "Custom")
        {
            CurrentReport.Page.ApplyCustomSize(PageWidthInput, PageHeightInput);
            CurrentReport.Page.Orientation = SelectedPageOrientation;
        }
        else
        {
            CurrentReport.Page.ApplyPreset(SelectedPaperName, SelectedPageOrientation);
            PageWidthInput = CurrentReport.Page.Width;
            PageHeightInput = CurrentReport.Page.Height;
        }

        SyncPageToDesigner();
        StatusMessage = $"Report size set to {CurrentReport.Page.PaperName} ({CurrentReport.Page.Width:0} x {CurrentReport.Page.Height:0})";
    }

    private void SyncPageToDesigner()
    {
        DesignViewModel.PageWidth = CurrentReport.Page.Width;
        DesignViewModel.PageHeight = CurrentReport.Page.Height;
    }

    private void SyncPageFromReport()
    {
        SelectedPaperName = CurrentReport.Page.PaperName;
        SelectedPageOrientation = CurrentReport.Page.Orientation;
        PageWidthInput = CurrentReport.Page.Width;
        PageHeightInput = CurrentReport.Page.Height;
        SyncPageToDesigner();
    }

    [RelayCommand]
    private void NewReport()
    {
        CurrentReport = new ReportTemplate { Name = "New Report" };
        DesignViewModel.Bands.Clear();
        DataSourceViewModel.DataSources.Clear();
        InitializeDefaultReport();
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
            try
            {
                var serializer = new FrxSerializer();
                var content = await File.ReadAllTextAsync(dialog.FileName);
                CurrentReport = serializer.Deserialize(content);

                // Sync to child viewmodels
                DesignViewModel.Bands = new ObservableCollection<Band>(CurrentReport.Bands);
                DataSourceViewModel.DataSources = new ObservableCollection<DataSource>(CurrentReport.DataSources);
                SyncPageFromReport();

                StatusMessage = $"Opened: {dialog.FileName}";
                OnPropertyChanged(nameof(ReportBands));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private async Task SaveReport()
    {
        // Sync from child viewmodels to main report
        CurrentReport.Bands = DesignViewModel.Bands.ToList();
        CurrentReport.DataSources = DataSourceViewModel.DataSources.ToList();

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
        if (CurrentReport.Bands.Count == 0)
        {
            MessageBox.Show("Report has no bands to preview.", "Preview", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var previewWindow = new Views.PreviewWindow(CurrentReport);
        previewWindow.Show();
        StatusMessage = "Preview opened";
    }

    [RelayCommand]
    private void DataSources()
    {
        var dialog = new Views.ConnectionDialog();
        if (dialog.ShowDialog() == true)
        {
            // Connection was established, refresh data sources
            StatusMessage = "Data source configured";
        }
    }

    [RelayCommand]
    private void Parameters()
    {
        MessageBox.Show("Parameters dialog would open here.\n\n" +
            "Current parameters:\n" + 
            string.Join("\n", CurrentReport.Parameters.Select(p => $"- {p.Name}: {p.DataType}")),
            "Parameters", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Styles()
    {
        MessageBox.Show("Styles editor would open here.", "Styles", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Print()
    {
        MessageBox.Show("Print dialog would open here.", "Print", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Undo()
    {
        StatusMessage = "Undo not implemented yet";
    }

    [RelayCommand]
    private void Redo()
    {
        StatusMessage = "Redo not implemented yet";
    }

    [RelayCommand]
    private void Cut()
    {
        StatusMessage = "Cut not implemented yet";
    }

    [RelayCommand]
    private void Copy()
    {
        StatusMessage = "Copy not implemented yet";
    }

    [RelayCommand]
    private void Paste()
    {
        StatusMessage = "Paste not implemented yet";
    }

    [RelayCommand]
    private void Delete()
    {
        if (DesignViewModel.SelectedObject != null)
        {
            DesignViewModel.RemoveObject(DesignViewModel.SelectedObject);
            StatusMessage = "Object deleted";
        }
        else if (DesignViewModel.SelectedBand != null)
        {
            DesignViewModel.RemoveBand(DesignViewModel.SelectedBand);
            StatusMessage = "Band deleted";
        }
    }

    [RelayCommand]
    private void Documentation()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://docs.microsoft.com",
                UseShellExecute = true
            });
        }
        catch { }
    }

    [RelayCommand]
    private void About()
    {
        MessageBox.Show("Report Designer v1.0\nBuilt with .NET 10 and WPF\n\n" +
            "Features:\n- Drag & Drop Design\n- SQL/SQLite Data Sources\n- " +
            "FastReport-compatible .frx format\n- MAUI Report Engine integration",
            "About Report Designer", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }
}

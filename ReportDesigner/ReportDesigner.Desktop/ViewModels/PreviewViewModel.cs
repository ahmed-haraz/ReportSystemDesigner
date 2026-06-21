using CommunityToolkit.Mvvm.ComponentModel;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;

namespace ReportDesigner.Desktop.ViewModels;

public partial class PreviewViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportTemplate _report;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private double _zoom = 1.0;

    [ObservableProperty]
    private ObservableCollection<PreviewPage> _pages = new();

    public PreviewViewModel(ReportTemplate report)
    {
        _report = report;
    }

    partial void OnCurrentPageChanged(int value)
    {
        // Update displayed page
    }
}

public class PreviewPage
{
    public int PageNumber { get; set; }
    public byte[]? ImageData { get; set; }
    public List<RenderedObject> Objects { get; set; } = new();
}

public class RenderedObject
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public string Text { get; set; } = "";
    public string FontName { get; set; } = "Arial";
    public float FontSize { get; set; } = 10;
    public bool Bold { get; set; }
}

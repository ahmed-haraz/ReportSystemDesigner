using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

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

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _hasData = false;

    // Sample data for preview before saving
    [ObservableProperty]
    private ObservableCollection<Dictionary<string, object>> _sampleData = new();


    public PreviewViewModel(ReportTemplate report)
    {
        _report = report;
        GenerateSampleData();
        _ = GeneratePreviewAsync();
    }

    partial void OnCurrentPageChanged(int value)
    {
        if (value >= 1 && value <= TotalPages)
        {
            // Update displayed page
            OnPropertyChanged(nameof(CurrentPreviewPage));
        }
    }


    public PreviewPage? CurrentPreviewPage =>
        Pages.FirstOrDefault(p => p.PageNumber == CurrentPage);

    // NEW: Generate sample data for preview
    private void GenerateSampleData()
    {
        SampleData.Clear();

        // Generate 5 sample rows based on data sources
        for (int i = 1; i <= 5; i++)
        {
            var row = new Dictionary<string, object>
            {
                ["ProductName"] = $"Sample Product {i}",
                ["Quantity"] = i * 10,
                ["Price"] = 99.99m * i,
                ["OrderDate"] = DateTime.Now.AddDays(-i),
                ["CustomerName"] = $"Customer {i}",
                ["OrderID"] = 1000 + i
            };
            SampleData.Add(row);
        }

        HasData = SampleData.Count > 0;
    }

    // NEW: Generate preview pages with actual rendering simulation
    [RelayCommand]
    private async Task GeneratePreviewAsync()
    {
        if (Report?.Bands == null || Report.Bands.Count == 0)
        {
            StatusMessage = "No bands to preview";
            return;
        }

        IsGenerating = true;
        StatusMessage = "Generating preview...";

        try
        {
            await Task.Delay(100); // Allow UI update

            Pages.Clear();

            // Simulate page generation
            var page = new PreviewPage
            {
                PageNumber = 1,
                Objects = new List<RenderedObject>()
            };

            float currentY = 0;

            foreach (var band in Report.Bands.Where(b => b.Visible && b.IsVisibleInDesigner))
            {
                // Render band background
                page.Objects.Add(new RenderedObject
                {
                    Left = 0,
                    Top = currentY,
                    Width = Report.Page?.Width ?? 595,
                    Height = band.Height,
                    Text = "",
                    IsBandBackground = true,
                    BandName = band.Name
                });

                // Render band objects
                foreach (var obj in band.Objects.Where(o => o.Visible))
                {
                    var renderedObj = new RenderedObject
                    {
                        Left = obj.Left,
                        Top = currentY + obj.Top,
                        Width = obj.Width,
                        Height = obj.Height,
                        Text = ResolveExpression(obj.Expression, obj.Text),
                        FontName = obj.TextProps?.FontName ?? "Arial",
                        FontSize = obj.TextProps?.FontSize ?? 10,
                        Bold = obj.TextProps?.Bold ?? false,
                        DataBinding = obj.DataBinding,
                        ObjectType = obj.Type.ToString()
                    };

                    // Special handling for table cells
                    if (obj.Type == ObjectType.Table && obj.TableProps != null)
                    {
                        renderedObj.TableCells = RenderTableCells(obj, currentY);
                    }

                    page.Objects.Add(renderedObj);
                }

                currentY += band.Height;
            }

            Pages.Add(page);
            TotalPages = 1;
            CurrentPage = 1;
            StatusMessage = $"Preview generated: {TotalPages} page(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Preview error: {ex.Message}";
            MessageBox.Show($"Error generating preview: {ex.Message}", "Preview Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsGenerating = false;
        }
    }

    // NEW: Resolve expressions with sample data
    private string ResolveExpression(string expression, string defaultText)
    {
        if (string.IsNullOrEmpty(expression))
            return defaultText;

        // Simple expression resolution for preview
        var result = expression;

        // Replace field references with sample data
        foreach (var data in SampleData.FirstOrDefault() ?? new Dictionary<string, object>())
        {
            result = result.Replace($"[{data.Key}]", data.Value?.ToString() ?? "");
        }

        // Replace system variables
        result = result.Replace("[Date]", DateTime.Now.ToShortDateString());
        result = result.Replace("[Page#]", "1");
        result = result.Replace("[TotalPages#]", "1");
        result = result.Replace("[Time]", DateTime.Now.ToShortTimeString());

        return result;
    }

    // NEW: Render table cells for preview
    private List<RenderedTableCell> RenderTableCells(ReportObject tableObj, float bandTop)
    {
        var cells = new List<RenderedTableCell>();

        if (tableObj.TableProps?.Rows == null) return cells;

        foreach (var row in tableObj.TableProps.Rows)
        {
            foreach (var cell in row.Cells)
            {
                var sampleValue = GetSampleValueForField(cell.DataField);

                cells.Add(new RenderedTableCell
                {
                    Left = tableObj.Left + cell.Left,
                    Top = bandTop + tableObj.Top + cell.Top,
                    Width = cell.Width,
                    Height = cell.Height,
                    Text = !string.IsNullOrEmpty(sampleValue) ? sampleValue : cell.Text,
                    IsHeader = cell.IsHeaderCell,
                    RowIndex = cell.RowIndex,
                    ColumnIndex = cell.ColumnIndex
                });
            }
        }

        return cells;
    }

    private string GetSampleValueForField(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName)) return "";

        var firstRow = SampleData.FirstOrDefault();
        if (firstRow != null && firstRow.ContainsKey(fieldName))
        {
            return firstRow[fieldName]?.ToString() ?? "";
        }

        return $"[{fieldName}]";
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
            CurrentPage++;
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
            CurrentPage--;
    }

    [RelayCommand]
    private void ZoomIn()
    {
        Zoom = Math.Min(Zoom + 0.25, 4.0);
    }

    [RelayCommand]
    private void ZoomOut()
    {
        Zoom = Math.Max(Zoom - 0.25, 0.25);
    }

    [RelayCommand]
    private void FitToWidth()
    {
        Zoom = 1.0; // Calculate based on viewport
    }

}
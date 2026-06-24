using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;

namespace ReportDesigner.Desktop.ViewModels;

public partial class ToolboxViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ToolboxItem> _bandTypes = new();

    [ObservableProperty]
    private ObservableCollection<ToolboxItem> _objectTypes = new();

    [ObservableProperty]
    private ToolboxItem? _selectedBandType;

    [ObservableProperty]
    private ToolboxItem? _selectedObjectType;

    [ObservableProperty]
    private bool _isDraggingBand;

    [ObservableProperty]
    private bool _isDraggingObject;

    public ToolboxViewModel()
    {
        InitializeBandTypes();
        InitializeObjectTypes();
    }

    private void InitializeBandTypes()
    {
        foreach (BandType type in Enum.GetValues(typeof(BandType)))
        {
            BandTypes.Add(new ToolboxItem
            {
                Name = type.ToString(),
                Type = type.ToString(),
                Icon = "/Icons/Band.png",
                IsBand = true,
                Description = GetBandDescription(type),
                CanHide = true
            });
        }
    }

    private void InitializeObjectTypes()
    {
        foreach (ObjectType type in Enum.GetValues(typeof(ObjectType)))
        {
            ObjectTypes.Add(new ToolboxItem
            {
                Name = type.ToString(),
                Type = type.ToString(),
                Icon = $"/Icons/{type}.png",
                IsBand = false,
                Description = GetObjectDescription(type),
                CanRemove = true
            });
        }
    }

    [RelayCommand]
    private void SelectBandType(string typeName)
    {
        SelectedBandType = BandTypes.FirstOrDefault(b => b.Type == typeName);
        SelectedObjectType = null;
    }

    [RelayCommand]
    private void SelectObjectType(string typeName)
    {
        SelectedObjectType = ObjectTypes.FirstOrDefault(o => o.Type == typeName);
        SelectedBandType = null;
    }

    [RelayCommand]
    private void RemoveItem(ToolboxItem? item)
    {
        if (item == null) return;

        if (item.IsBand)
        {
            var bandItem = BandTypes.FirstOrDefault(b => b.Type == item.Type);
            if (bandItem != null)
            {
                bandItem.IsVisible = false;
                BandTypes.Remove(bandItem);
            }
        }
        else
        {
            var objItem = ObjectTypes.FirstOrDefault(o => o.Type == item.Type);
            if (objItem != null)
            {
                objItem.IsVisible = false;
                ObjectTypes.Remove(objItem);
            }
        }
    }

    [RelayCommand]
    private void OpenTableDesigner()
    {
        // Open table designer dialog
        if (SelectedObjectType?.Type == "Table")
        {
            // Table designer would open here
        }
    }

    public void StartBandDrag(string typeName)
    {
        SelectedBandType = BandTypes.FirstOrDefault(b => b.Type == typeName);
        IsDraggingBand = true;
        IsDraggingObject = false;
    }

    public void StartObjectDrag(string typeName)
    {
        SelectedObjectType = ObjectTypes.FirstOrDefault(o => o.Type == typeName);
        IsDraggingObject = true;
        IsDraggingBand = false;
    }

    private string GetBandDescription(BandType type)
    {
        return type switch
        {
            BandType.ReportTitle => "Report header shown once at the top",
            BandType.ReportSummary => "Report footer shown once at the end",
            BandType.PageHeader => "Header repeated on every page",
            BandType.PageFooter => "Footer repeated on every page",
            BandType.Data => "Main data rows, repeated for each record",
            BandType.ColumnHeader => "Column headers for tables",
            BandType.GroupHeader => "Header for grouped data",
            BandType.GroupFooter => "Footer for grouped data",
            _ => "Band"
        };
    }

    private string GetObjectDescription(ObjectType type)
    {
        return type switch
        {
            ObjectType.Text => "Text with data binding support",
            ObjectType.Picture => "Image from file or data",
            ObjectType.Line => "Horizontal or vertical line",
            ObjectType.Shape => "Rectangle, ellipse, etc.",
            ObjectType.Barcode => "Barcode (Code128, QR, etc.)",
            ObjectType.Table => "Data-bound table with cells",
            ObjectType.Chart => "Chart/graph",
            ObjectType.Subreport => "Nested report",
            _ => "Object"
        };
    }
}

public class ToolboxItem
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Icon { get; set; } = "";
    public bool IsBand { get; set; }
    public string Description { get; set; } = "";
    public bool CanHide { get; set; }
    public bool CanRemove { get; set; }
    public bool IsVisible { get; set; } = true;

    public ObjectType ObjectType => Enum.Parse<ObjectType>(Type);
    public BandType BandType => Enum.Parse<BandType>(Type);
}

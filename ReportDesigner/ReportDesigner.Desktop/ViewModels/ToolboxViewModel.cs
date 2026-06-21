using CommunityToolkit.Mvvm.ComponentModel;
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
                IsBand = true
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
                IsBand = false
            });
        }
    }
}

public class ToolboxItem
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Icon { get; set; } = "";
    public bool IsBand { get; set; }
    public ObjectType ObjectType => Enum.Parse<ObjectType>(Type);
    public BandType BandType => Enum.Parse<BandType>(Type);
}

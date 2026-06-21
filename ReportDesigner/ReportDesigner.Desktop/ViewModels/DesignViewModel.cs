using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;

namespace ReportDesigner.Desktop.ViewModels;

public partial class DesignViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Band> _bands = new();

    [ObservableProperty]
    private Band? _selectedBand;

    [ObservableProperty]
    private ReportObject? _selectedObject;

    [ObservableProperty]
    private float _pageWidth = 595;

    [ObservableProperty]
    private float _pageHeight = 842;

    [ObservableProperty]
    private float _zoom = 1.0f;

    [RelayCommand]
    public void SelectObject(ReportObject obj)
    {
        SelectedObject = obj;
    }

    [RelayCommand]
    private void AddBand(BandType type)
    {
        var band = new Band
        {
            Type = type,
            Name = $"{type}Band{Bands.Count + 1}",
            Top = Bands.Any() ? Bands.Max(b => b.Top + b.Height) + 10 : 0
        };
        Bands.Add(band);
    }

    [RelayCommand]
    private void RemoveBand(Band band)
    {
        Bands.Remove(band);
    }

    [RelayCommand]
    private void AddObject(ObjectType type)
    {
        if (SelectedBand == null) return;

        var obj = new ReportObject
        {
            Type = type,
            Name = $"{type}Object{SelectedBand.Objects.Count + 1}",
            Left = 10,
            Top = 5,
            Width = 100,
            Height = 20
        };
        SelectedBand.Objects.Add(obj);
    }

    [RelayCommand]
    private void RemoveObject(ReportObject obj)
    {
        SelectedBand?.Objects.Remove(obj);
    }
}

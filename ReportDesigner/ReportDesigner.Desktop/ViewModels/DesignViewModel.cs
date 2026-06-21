using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportDesigner.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

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

    [ObservableProperty]
    private bool _isDragging;

    [ObservableProperty]
    private Point _dragStart;

    [ObservableProperty]
    private Point _dragCurrent;

    partial void OnSelectedBandChanged(Band? value)
    {
        if (value != null)
        {
            SelectedObject = null;
        }
    }

    partial void OnSelectedObjectChanged(ReportObject? value)
    {
        if (value != null && value.TextProps != null)
        {
            OnPropertyChanged(nameof(SelectedObject));
        }
    }

    [RelayCommand]
    public void SelectBand(Band band)
    {
        SelectedBand = band;
        foreach (var b in Bands)
        {
            b.IsSelected = (b == band);
        }
    }

    [RelayCommand]
    public void SelectObject(ReportObject obj)
    {
        SelectedObject = obj;
    }

    [RelayCommand]
    public void AddBand(BandType type)
    {
        var band = new Band
        {
            Type = type,
            Name = $"{type}Band{Bands.Count(b => b.Type == type) + 1}",
            Top = CalculateBandTop(),
            Height = GetDefaultBandHeight(type)
        };

        Bands.Add(band);
        SelectedBand = band;

        RecalculateBandPositions();
    }

    [RelayCommand]
    public void RemoveBand(Band? band)
    {
        if (band == null) return;

        Bands.Remove(band);
        if (SelectedBand == band)
        {
            SelectedBand = Bands.FirstOrDefault();
        }

        RecalculateBandPositions();
    }

    [RelayCommand]
    private void AddObject(ObjectType type)
    {
        if (SelectedBand == null)
        {
            MessageBox.Show("Please select a band first.", "Add Object", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var obj = new ReportObject
        {
            Type = type,
            Name = $"{type}Object{SelectedBand.Objects.Count + 1}",
            Left = 10,
            Top = 5,
            Width = GetDefaultObjectWidth(type),
            Height = GetDefaultObjectHeight(type),
            TextProps = new TextProperties()
        };

        switch (type)
        {
            case ObjectType.Text:
                obj.Text = "Text";
                break;
            case ObjectType.Picture:
                obj.Text = "[Picture]";
                obj.PictureProps = new PictureProperties();
                break;
            case ObjectType.Barcode:
                obj.Text = "[Barcode]";
                obj.BarcodeProps = new BarcodeProperties();
                break;
            case ObjectType.Line:
                obj.Width = 100;
                obj.Height = 1;
                break;
            case ObjectType.Shape:
                obj.ShapeProps = new ShapeProperties();
                break;
            case ObjectType.Table:
                obj.TableProps = new TableProperties();
                break;
        }

        SelectedBand.Objects.Add(obj);
        SelectedObject = obj;

        OnPropertyChanged(nameof(Bands));
    }

    [RelayCommand]
    public void RemoveObject(ReportObject? obj)
    {
        if (obj == null || SelectedBand == null) return;

        SelectedBand.Objects.Remove(obj);
        if (SelectedObject == obj)
        {
            SelectedObject = SelectedBand.Objects.FirstOrDefault();
        }

        OnPropertyChanged(nameof(Bands));
    }

    // Manual methods (not RelayCommand) - called directly from code-behind
    public void MoveObject(ReportObject obj, double deltaX, double deltaY)
    {
        obj.Left += (float)deltaX;
        obj.Top += (float)deltaY;

        if (obj.Left < 0) obj.Left = 0;
        if (obj.Top < 0) obj.Top = 0;
        if (obj.Left + obj.Width > PageWidth) obj.Left = PageWidth - obj.Width;
        if (obj.Top + obj.Height > SelectedBand?.Height) obj.Top = (SelectedBand?.Height ?? 0) - obj.Height;
    }

    public void ResizeObject(ReportObject obj, double newWidth, double newHeight)
    {
        obj.Width = (float)Math.Max(10, newWidth);
        obj.Height = (float)Math.Max(10, newHeight);
    }

    public void StartDrag(Point position)
    {
        IsDragging = true;
        DragStart = position;
    }

    public void DoDrag(Point currentPosition)
    {
        if (!IsDragging || SelectedObject == null) return;

        var deltaX = (currentPosition.X - DragStart.X) / Zoom;
        var deltaY = (currentPosition.Y - DragStart.Y) / Zoom;

        SelectedObject.Left += (float)deltaX;
        SelectedObject.Top += (float)deltaY;

        DragStart = currentPosition;
    }

    public void EndDrag()
    {
        IsDragging = false;
    }

    public void DropObject(ObjectType type, Point position)
    {
        if (SelectedBand == null) return;

        var obj = new ReportObject
        {
            Type = type,
            Name = $"{type}Object{SelectedBand.Objects.Count + 1}",
            Left = (float)(position.X / Zoom),
            Top = (float)(position.Y / Zoom),
            Width = GetDefaultObjectWidth(type),
            Height = GetDefaultObjectHeight(type),
            TextProps = new TextProperties()
        };

        SelectedBand.Objects.Add(obj);
        SelectedObject = obj;

        OnPropertyChanged(nameof(Bands));
    }

    public void DropBand(BandType type, double position)
    {
        var band = new Band
        {
            Type = type,
            Name = $"{type}Band{Bands.Count(b => b.Type == type) + 1}",
            Top = (float)(position / Zoom),
            Height = GetDefaultBandHeight(type)
        };

        Bands.Add(band);
        SelectedBand = band;

        RecalculateBandPositions();
    }

    private float CalculateBandTop()
    {
        if (!Bands.Any()) return 0;
        return Bands.Max(b => b.Top + b.Height) + 10;
    }

    private void RecalculateBandPositions()
    {
        float currentTop = 0;
        foreach (var band in Bands.OrderBy(b => b.Top))
        {
            band.Top = currentTop;
            currentTop += band.Height + 2;
        }

        OnPropertyChanged(nameof(Bands));
    }

    private float GetDefaultBandHeight(BandType type)
    {
        return type switch
        {
            BandType.ReportTitle => 80,
            BandType.PageHeader => 25,
            BandType.Data => 20,
            BandType.PageFooter => 30,
            BandType.ReportSummary => 40,
            BandType.ColumnHeader => 25,
            BandType.GroupHeader => 30,
            BandType.GroupFooter => 30,
            _ => 30
        };
    }

    private float GetDefaultObjectWidth(ObjectType type)
    {
        return type switch
        {
            ObjectType.Text => 100,
            ObjectType.Picture => 80,
            ObjectType.Barcode => 120,
            ObjectType.Line => 100,
            ObjectType.Shape => 60,
            ObjectType.Table => 300,
            _ => 100
        };
    }

    private float GetDefaultObjectHeight(ObjectType type)
    {
        return type switch
        {
            ObjectType.Text => 20,
            ObjectType.Picture => 60,
            ObjectType.Barcode => 50,
            ObjectType.Line => 1,
            ObjectType.Shape => 40,
            ObjectType.Table => 100,
            _ => 20
        };
    }
}

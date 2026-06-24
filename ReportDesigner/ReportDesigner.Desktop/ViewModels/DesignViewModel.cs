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
    private bool _snapToGrid = true;

    [ObservableProperty]
    private float _gridSize = 5;

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
        foreach (var band in Bands)
        {
            foreach (var reportObject in band.Objects)
            {
                reportObject.IsSelected = ReferenceEquals(reportObject, obj);
            }
        }

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

        var obj = CreateDefaultObject(type, SelectedBand.Objects.Count + 1);

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
        obj.Left = Snap((float)(obj.Left + deltaX));
        obj.Top = Snap((float)(obj.Top + deltaY));
        ClampObjectToBand(obj, SelectedBand);
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
        var pageX = (float)(position.X / Zoom);
        var pageY = (float)(position.Y / Zoom);
        var targetBand = FindBandAt(pageY) ?? SelectedBand;
        if (targetBand == null) return;

        SelectBand(targetBand);

        var obj = CreateDefaultObject(type, targetBand.Objects.Count + 1);
        obj.Left = Snap(pageX);
        obj.Top = Snap(pageY - targetBand.Top);
        ClampObjectToBand(obj, targetBand);

        targetBand.Objects.Add(obj);
        SelectedObject = obj;

        OnPropertyChanged(nameof(Bands));
    }

    public void DropDataField(string dataBinding, string caption, Point position)
    {
        DropObject(ObjectType.Text, position);
        if (SelectedObject == null) return;

        SelectedObject.Text = string.IsNullOrWhiteSpace(caption) ? dataBinding : caption;
        SelectedObject.DataBinding = dataBinding;
        SelectedObject.Expression = string.IsNullOrWhiteSpace(dataBinding) ? string.Empty : $"[{dataBinding}]";
        SelectedObject.Name = $"Field_{caption.Replace(" ", "_").Replace(".", "_")}";
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

    private ReportObject CreateDefaultObject(ObjectType type, int index)
    {
        var obj = new ReportObject
        {
            Type = type,
            Name = $"{type}Object{index}",
            Left = 10,
            Top = 5,
            Width = GetDefaultObjectWidth(type),
            Height = GetDefaultObjectHeight(type),
            TextProps = new TextProperties(),
            Text = type == ObjectType.Text ? "Text" : $"[{type}]"
        };

        if (type == ObjectType.Picture) obj.PictureProps = new PictureProperties();
        if (type == ObjectType.Barcode) obj.BarcodeProps = new BarcodeProperties();
        if (type == ObjectType.Shape) obj.ShapeProps = new ShapeProperties();
        if (type == ObjectType.Table) obj.TableProps = new TableProperties();

        return obj;
    }

    private Band? FindBandAt(float pageY) =>
        Bands.OrderBy(b => b.Top).FirstOrDefault(b => pageY >= b.Top && pageY <= b.Top + b.Height);

    private float Snap(float value) => SnapToGrid && GridSize > 0
        ? MathF.Round(value / GridSize) * GridSize
        : value;

    private void ClampObjectToBand(ReportObject obj, Band? band)
    {
        if (band == null) return;
        obj.Left = Math.Clamp(obj.Left, 0, Math.Max(0, PageWidth - obj.Width));
        obj.Top = Math.Clamp(obj.Top, -band.Top, Math.Max(-band.Top, PageHeight - band.Top - obj.Height));
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

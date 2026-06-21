using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReportDesigner.Core.Models;
using ReportDesigner.Desktop.ViewModels;

namespace ReportDesigner.Desktop.Views;

public partial class DesignSurface : UserControl
{
    private bool _isDragging;
    private Point _dragStart;
    private ReportObject? _selectedObject;

    public DesignSurface()
    {
        InitializeComponent();
    }

    private void DesignCanvas_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(ToolboxItem)) || e.Data.GetDataPresent(typeof(BandType)))
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
    }

    private void DesignCanvas_Drop(object sender, DragEventArgs e)
    {
        // Handle drop from toolbox
    }

    private void DesignCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Selection logic
    }

    private void DesignCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && _selectedObject != null)
        {
            var current = e.GetPosition(DesignCanvas);
            var delta = current - _dragStart;
            _selectedObject.Left += (float)delta.X;
            _selectedObject.Top += (float)delta.Y;
            _dragStart = current;
        }
    }

    private void DesignCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            _selectedObject = null;
            Mouse.Capture(null);
        }
    }

    private void Object_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is ReportObject obj)
        {
            _selectedObject = obj;
            _isDragging = true;
            _dragStart = e.GetPosition(DesignCanvas);
            border.CaptureMouse();

            if (DataContext is DesignViewModel vm)
            {
                vm.SelectObject(obj);
            }
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

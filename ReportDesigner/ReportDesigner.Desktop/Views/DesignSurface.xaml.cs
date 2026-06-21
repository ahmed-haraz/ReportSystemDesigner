using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReportDesigner.Core.Models;
using ReportDesigner.Desktop.ViewModels;

namespace ReportDesigner.Desktop.Views;

public partial class DesignSurface : UserControl
{
    private bool _isDraggingObject;
    private Point _dragStartPoint;
    private ReportObject? _draggedObject;
    private Point _objectStartPosition;
    private Border? _draggedBorder;

    public DesignSurface()
    {
        InitializeComponent();

        // Attach drop to Canvas directly
        DesignCanvas.Drop += DesignCanvas_Drop;
        DesignCanvas.DragOver += DesignCanvas_DragOver;
        DesignCanvas.PreviewMouseLeftButtonDown += DesignCanvas_PreviewMouseLeftButtonDown;
        DesignCanvas.PreviewMouseMove += DesignCanvas_PreviewMouseMove;
        DesignCanvas.PreviewMouseLeftButtonUp += DesignCanvas_PreviewMouseLeftButtonUp;
    }

    // ====== DROP FROM TOOLBOX ======

    private void DesignCanvas_DragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("BandType") || e.Data.GetDataPresent("ObjectType"))
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void DesignCanvas_Drop(object? sender, DragEventArgs e)
    {
        var position = e.GetPosition(DesignCanvas);
        var vm = DataContext as DesignViewModel;
        if (vm == null) return;

        var adjustedX = position.X / vm.Zoom;
        var adjustedY = position.Y / vm.Zoom;

        if (e.Data.GetData("BandType") is BandType bandType)
        {
            vm.DropBand(bandType, adjustedY);
            e.Handled = true;
        }
        else if (e.Data.GetData("ObjectType") is ObjectType objectType)
        {
            if (vm.SelectedBand == null)
            {
                MessageBox.Show("Please select a band first by clicking on it, then drag objects onto it.", 
                    "Add Object", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            vm.DropObject(objectType, new Point(adjustedX, adjustedY));
            e.Handled = true;
        }
    }

    // ====== BAND SELECTION ======

    private void BandBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is Band band)
        {
            var vm = DataContext as DesignViewModel;
            vm?.SelectBand(band);
            e.Handled = true;
        }
    }

    // ====== OBJECT SELECTION AND DRAG ======

    private void ObjectBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is ReportObject obj)
        {
            _isDraggingObject = true;
            _draggedObject = obj;
            _draggedBorder = border;
            _dragStartPoint = e.GetPosition(DesignCanvas);
            _objectStartPosition = new Point(obj.Left, obj.Top);

            var vm = DataContext as DesignViewModel;
            vm?.SelectObject(obj);

            border.CaptureMouse();
            e.Handled = true;
        }
    }

    private void ObjectBorder_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDraggingObject && _draggedObject != null && _draggedBorder != null)
        {
            var currentPosition = e.GetPosition(DesignCanvas);
            var vm = DataContext as DesignViewModel;
            if (vm == null) return;

            var deltaX = (currentPosition.X - _dragStartPoint.X) / vm.Zoom;
            var deltaY = (currentPosition.Y - _dragStartPoint.Y) / vm.Zoom;

            var newLeft = _objectStartPosition.X + deltaX;
            var newTop = _objectStartPosition.Y + deltaY;

            // Keep within bounds
            if (newLeft < 0) newLeft = 0;
            if (newTop < 0) newTop = 0;
            if (newLeft + _draggedObject.Width > vm.PageWidth) newLeft = vm.PageWidth - _draggedObject.Width;
            if (vm.SelectedBand != null && newTop + _draggedObject.Height > vm.SelectedBand.Height)
                newTop = vm.SelectedBand.Height - _draggedObject.Height;

            _draggedObject.Left = (float)newLeft;
            _draggedObject.Top = (float)newTop;

            // Update Canvas position directly
            _draggedBorder.SetValue(Canvas.LeftProperty, newLeft);
            _draggedBorder.SetValue(Canvas.TopProperty, newTop);

            // Update properties panel via main VM
            UpdatePropertiesPanel();
        }
    }

    private void ObjectBorder_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDraggingObject)
        {
            _isDraggingObject = false;
            _draggedObject = null;

            if (_draggedBorder != null)
            {
                _draggedBorder.ReleaseMouseCapture();
                _draggedBorder = null;
            }
        }
    }

    // ====== CANVAS CLICK (Deselect) ======

    private void DesignCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Only deselect if clicking directly on canvas background
        if (e.OriginalSource == DesignCanvas || e.OriginalSource is DrawingBrush)
        {
            var vm = DataContext as DesignViewModel;
            if (vm?.SelectedObject != null)
            {
                vm.SelectObject(null!);
            }
        }
    }

    private void DesignCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(DesignCanvas);
        var vm = DataContext as DesignViewModel;
        if (vm != null)
        {
            var mainVm = FindParentViewModel();
            if (mainVm != null)
            {
                mainVm.CursorPosition = $"X: {position.X / vm.Zoom:F1}, Y: {position.Y / vm.Zoom:F1}";
            }
        }
    }

    private void DesignCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    private void UpdatePropertiesPanel()
    {
        var mainVm = FindParentViewModel();
        if (mainVm != null && _draggedObject != null)
        {
            // Update properties view model directly
            mainVm.PropertiesViewModel.SelectedObject = _draggedObject;
            mainVm.PropertiesViewModel.LoadObjectProperties(_draggedObject);
        }
    }

    private MainViewModel? FindParentViewModel()
    {
        var window = Window.GetWindow(this);
        if (window?.DataContext is MainViewModel vm)
        {
            return vm;
        }
        return null;
    }
}

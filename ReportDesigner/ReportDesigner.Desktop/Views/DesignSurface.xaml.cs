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
    }

    // ====== DROP FROM TOOLBOX ======

    private void DesignCanvas_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("BandType") || e.Data.GetDataPresent("ObjectType") || e.Data.GetDataPresent("DataColumn"))
        {
            e.Effects = e.Data.GetDataPresent("DataColumn") ? DragDropEffects.Copy : DragDropEffects.Move;
            e.Handled = true;
        }
    }

    private void DesignCanvas_Drop(object sender, DragEventArgs e)
    {
        var position = e.GetPosition(DesignCanvas);
        var vm = DataContext as DesignViewModel;
        if (vm == null) return;

        if (e.Data.GetData("BandType") is BandType bandType)
        {
            vm.DropBand(bandType, position.Y);
            e.Handled = true;
        }
        else if (e.Data.GetDataPresent("DataColumn"))
        {
            var binding = e.Data.GetData("DataColumn")?.ToString() ?? string.Empty;
            var caption = e.Data.GetData("DataColumnName")?.ToString() ?? binding;
            vm.DropDataField(binding, caption, position);
            e.Handled = true;
        }
        else if (e.Data.GetData("ObjectType") is ObjectType objectType)
        {
            vm.DropObject(objectType, position);
            e.Handled = true;
        }
    }

    // ====== BAND SELECTION ======

    private void BandBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is Band band)
        {
            var vm = DataContext as DesignViewModel;
            if (vm != null)
            {
                // Deselect all bands
                foreach (var b in vm.Bands)
                {
                    b.IsSelected = false;
                }

                // Select clicked band
                band.IsSelected = true;
                vm.SelectBand(band);

                // Refresh band visuals
                BandsItemsControl.Items.Refresh();
            }

            e.Handled = true;
        }
    }

    // ====== OBJECT SELECTION AND DRAG ======

    private void ObjectBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is ReportObject obj)
        {
            _isDraggingObject = true;
            _draggedObject = obj;
            _draggedBorder = border;
            _dragStartPoint = e.GetPosition(DesignCanvas);
            _objectStartPosition = new Point(obj.Left, obj.Top);

            var vm = DataContext as DesignViewModel;
            if (vm != null)
            {
                vm.SelectObject(obj);
                UpdatePropertiesPanel(obj);
            }

            border.CaptureMouse();
            e.Handled = true;
        }
    }

    private void ObjectBorder_MouseMove(object sender, MouseEventArgs e)
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

            if (vm.SnapToGrid && vm.GridSize > 0)
            {
                newLeft = Math.Round(newLeft / vm.GridSize) * vm.GridSize;
                newTop = Math.Round(newTop / vm.GridSize) * vm.GridSize;
            }

            // Keep inside the page, but allow free movement across band boundaries.
            var selectedBandTop = vm.SelectedBand?.Top ?? 0;
            var minTop = -selectedBandTop;
            var maxTop = vm.PageHeight - selectedBandTop - _draggedObject.Height;
            if (newLeft < 0) newLeft = 0;
            if (newLeft + _draggedObject.Width > vm.PageWidth)
                newLeft = vm.PageWidth - _draggedObject.Width;
            if (newTop < minTop) newTop = minTop;
            if (newTop > maxTop) newTop = maxTop;

            _draggedObject.Left = (float)newLeft;
            _draggedObject.Top = (float)newTop;

            // Update properties panel live
            UpdatePropertiesPanel(_draggedObject);

            e.Handled = true;
        }
    }

    private void ObjectBorder_MouseLeftButtonUp(object sender, MouseEventArgs e)
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

            e.Handled = true;
        }
    }

    // ====== UPDATE PROPERTIES PANEL ======

    private void UpdatePropertiesPanel(ReportObject obj)
    {
        var mainVm = FindParentViewModel();
        if (mainVm != null)
        {
            mainVm.PropertiesViewModel.SelectedObject = obj;
            mainVm.PropertiesViewModel.LoadObjectProperties(obj);
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

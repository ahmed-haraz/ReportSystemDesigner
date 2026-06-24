using ReportDesigner.Core.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportDesigner.Desktop.Views;

public partial class ToolboxPanel : UserControl
{
    private Point _dragStartPoint;
    private bool _isDragging;

    public ToolboxPanel()
    {
        InitializeComponent();
    }

    // ====== BAND DRAG ======
    private void BandListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _isDragging = false;
    }

    private void BandListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            var currentPosition = e.GetPosition(null);
            if (Math.Abs(currentPosition.X - _dragStartPoint.X) > 5 ||
                Math.Abs(currentPosition.Y - _dragStartPoint.Y) > 5)
            {
                _isDragging = true;

                if (BandListBox.SelectedItem is ToolboxItem item)
                {
                    var data = new DataObject("BandType", item.BandType);
                    DragDrop.DoDragDrop(BandListBox, data, DragDropEffects.Move);
                    _isDragging = false;
                }
            }
        }
    }

    // ====== OBJECT DRAG ======
    private void ObjectListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _isDragging = false;
    }

    private void ObjectListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            var currentPosition = e.GetPosition(null);
            if (Math.Abs(currentPosition.X - _dragStartPoint.X) > 5 ||
                Math.Abs(currentPosition.Y - _dragStartPoint.Y) > 5)
            {
                _isDragging = true;

                if (ObjectListBox.SelectedItem is ToolboxItem item)
                {
                    var data = new DataObject("ObjectType", item.ObjectType);
                    DragDrop.DoDragDrop(ObjectListBox, data, DragDropEffects.Move);
                    _isDragging = false;
                }
            }
        }
    }
}

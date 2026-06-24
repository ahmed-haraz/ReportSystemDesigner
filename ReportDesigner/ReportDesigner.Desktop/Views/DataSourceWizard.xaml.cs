using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReportDesigner.Core.Models;
using ReportDesigner.Desktop.ViewModels;

namespace ReportDesigner.Desktop.Views;

public partial class DataSourceWizard : UserControl
{
    private Point _dragStartPoint;
    private bool _isDraggingColumn;

    public DataSourceWizard()
    {
        InitializeComponent();
    }

    private void TablesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is string tableName && DataContext is DataSourceViewModel vm)
        {
            vm.SelectTableCommand.Execute(tableName);
        }
    }

    private void ColumnsListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _isDraggingColumn = false;

        if ((e.OriginalSource as FrameworkElement)?.DataContext is DataColumn column)
        {
            ColumnsListBox.SelectedItem = column;
        }
    }

    private void ColumnsListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _isDraggingColumn) return;

        var currentPosition = e.GetPosition(null);
        if (Math.Abs(currentPosition.X - _dragStartPoint.X) <= 5 &&
            Math.Abs(currentPosition.Y - _dragStartPoint.Y) <= 5)
        {
            return;
        }

        if (ColumnsListBox.SelectedItem is not DataColumn column || DataContext is not DataSourceViewModel vm)
        {
            return;
        }

        _isDraggingColumn = true;
        var sourceName = vm.SelectedDataSource?.Name ?? "DataSource";
        var bindingPath = $"{sourceName}.{column.Name}";
        var data = new DataObject();
        data.SetData("DataColumn", bindingPath);
        data.SetData("DataColumnName", column.Caption.Length > 0 ? column.Caption : column.Name);
        DragDrop.DoDragDrop(ColumnsListBox, data, DragDropEffects.Copy);
        _isDraggingColumn = false;
    }
}

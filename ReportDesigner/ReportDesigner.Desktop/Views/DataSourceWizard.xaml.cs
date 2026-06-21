using System.Windows;
using System.Windows.Controls;
using ReportDesigner.Desktop.ViewModels;

namespace ReportDesigner.Desktop.Views;

public partial class DataSourceWizard : UserControl
{
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
}

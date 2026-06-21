using System.Windows;

namespace ReportDesigner.Desktop.Views;

public partial class QueryBuilder : Window
{
    public QueryBuilder()
    {
        InitializeComponent();
    }

    private void Execute_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Query execution would go here", "Execute", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

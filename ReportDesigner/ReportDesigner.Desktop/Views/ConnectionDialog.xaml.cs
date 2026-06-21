using System.Windows;

namespace ReportDesigner.Desktop.Views;

public partial class ConnectionDialog : Window
{
    public ConnectionDialog()
    {
        InitializeComponent();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "SQLite files (*.db;*.sqlite;*.sqlite3)|*.db;*.sqlite;*.sqlite3|All files (*.*)|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            txtDbPath.Text = dialog.FileName;
        }
    }

    private void TestSQLite_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Connection test would go here", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void TestSqlServer_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Connection test would go here", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
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

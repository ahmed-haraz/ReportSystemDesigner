using System.Windows;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace ReportDesigner.Desktop.Views;

public partial class ConnectionDialog : Window
{
    public string? SelectedDataSourceType { get; private set; }
    public string? ConnectionString { get; private set; }
    public string? SelectCommand { get; private set; }

    public ConnectionDialog()
    {
        InitializeComponent();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "SQLite files (*.db;*.sqlite;*.sqlite3)|*.db;*.sqlite;*.sqlite3|All files (*.*)|*.*",
            Title = "Select SQLite Database"
        };

        if (dialog.ShowDialog() == true)
        {
            txtDbPath.Text = dialog.FileName;
        }
    }

    private void TestSQLite_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = txtDbPath.Text,
                Mode = chkReadOnly.IsChecked == true ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite
            };

            if (!string.IsNullOrEmpty(txtDbPassword.Password))
            {
                builder.Password = txtDbPassword.Password;
            }

            using var connection = new SqliteConnection(builder.ConnectionString);
            connection.Open();

            txtStatus.Text = "Connection successful!";
            txtStatus.Foreground = System.Windows.Media.Brushes.Green;
            ConnectionString = builder.ConnectionString;
            SelectedDataSourceType = "SQLite";
        }
        catch (Exception ex)
        {
            txtStatus.Text = $"Connection failed: {ex.Message}";
            txtStatus.Foreground = System.Windows.Media.Brushes.Red;
        }
    }

    private void TestSqlServer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = txtServer.Text,
                InitialCatalog = txtDatabase.Text,
                IntegratedSecurity = chkWindowsAuth.IsChecked == true,
                ConnectTimeout = int.TryParse(txtTimeout.Text, out var timeout) ? timeout : 30
            };

            if (chkWindowsAuth.IsChecked != true)
            {
                builder.UserID = txtUsername.Text;
                builder.Password = txtPassword.Password;
            }

            using var connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            txtStatus.Text = "Connection successful!";
            txtStatus.Foreground = System.Windows.Media.Brushes.Green;
            txtSqlConnectionString.Text = builder.ConnectionString;
            ConnectionString = builder.ConnectionString;
            SelectedDataSourceType = "SqlServer";
        }
        catch (Exception ex)
        {
            txtStatus.Text = $"Connection failed: {ex.Message}";
            txtStatus.Foreground = System.Windows.Media.Brushes.Red;
        }
    }

    private void WindowsAuth_Checked(object sender, RoutedEventArgs e)
    {
        txtUsername.IsEnabled = false;
        txtPassword.IsEnabled = false;
    }

    private void WindowsAuth_Unchecked(object sender, RoutedEventArgs e)
    {
        txtUsername.IsEnabled = true;
        txtPassword.IsEnabled = true;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedTab = ConnectionTabs.SelectedItem as TabItem;

        if (selectedTab?.Header?.ToString() == "SQLite")
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = txtDbPath.Text
            };
            ConnectionString = builder.ConnectionString;
            SelectedDataSourceType = "SQLite";
        }
        else if (selectedTab?.Header?.ToString() == "SQL Server")
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = txtServer.Text,
                InitialCatalog = txtDatabase.Text,
                IntegratedSecurity = chkWindowsAuth.IsChecked == true
            };

            if (chkWindowsAuth.IsChecked != true)
            {
                builder.UserID = txtUsername.Text;
                builder.Password = txtPassword.Password;
            }

            ConnectionString = builder.ConnectionString;
            SelectedDataSourceType = "SqlServer";
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

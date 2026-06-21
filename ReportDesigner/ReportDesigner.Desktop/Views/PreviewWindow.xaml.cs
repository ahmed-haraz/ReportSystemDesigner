using System.Windows;
using ReportDesigner.Core.Models;

namespace ReportDesigner.Desktop.Views;

public partial class PreviewWindow : Window
{
    public PreviewWindow(ReportTemplate report)
    {
        InitializeComponent();
        // Set DataContext to PreviewViewModel
    }
}

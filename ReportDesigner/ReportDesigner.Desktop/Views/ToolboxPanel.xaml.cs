using System.Windows.Controls;
using System.Windows.Input;

namespace ReportDesigner.Desktop.Views;

public partial class ToolboxPanel : UserControl
{
    public ToolboxPanel()
    {
        InitializeComponent();
    }

    private void BandListBox_MouseMove(object sender, MouseEventArgs e)
    {
        // Drag start logic for bands
    }

    private void ObjectListBox_MouseMove(object sender, MouseEventArgs e)
    {
        // Drag start logic for objects
    }
}

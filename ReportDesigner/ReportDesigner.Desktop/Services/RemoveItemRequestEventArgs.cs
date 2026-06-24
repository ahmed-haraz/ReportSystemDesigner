using ReportDesigner.Core.Models;

namespace ReportDesigner.Desktop.Services;

public class RemoveItemRequestEventArgs : EventArgs
{
    public ToolboxItem Item { get; set; } = null!;
    public bool IsBand { get; set; }
}
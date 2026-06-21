using ReportDesigner.Core.Models;
using System.Windows.Media.Imaging;

namespace ReportDesigner.Desktop.Services;

public class PreviewService
{
    public BitmapSource? RenderPage(ReportTemplate template, int pageNumber, object? data = null)
    {
        // Render single page to bitmap for preview
        // This is a simplified implementation
        return null;
    }

    public List<BitmapSource?> RenderAllPages(ReportTemplate template, object? data = null)
    {
        var pages = new List<BitmapSource?>();
        // Render all pages
        return pages;
    }
}

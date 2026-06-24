namespace ReportDesigner.Core.Models;

public class PreviewPage
{
    public int PageNumber { get; set; }
    public byte[]? ImageData { get; set; }
    public List<RenderedObject> Objects { get; set; } = new();
}
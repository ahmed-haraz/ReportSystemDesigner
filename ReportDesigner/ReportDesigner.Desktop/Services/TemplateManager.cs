using ReportDesigner.Core.Models;
using ReportDesigner.Core.Parsers;
using System.IO;

namespace ReportDesigner.Desktop.Services;

public class TemplateManager
{
    private readonly FrxSerializer _serializer = new();
    private readonly string _templatesDirectory;

    public TemplateManager()
    {
        _templatesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ReportDesigner", "Templates");
        Directory.CreateDirectory(_templatesDirectory);
    }

    public async Task SaveTemplateAsync(ReportTemplate template, string? fileName = null)
    {
        fileName ??= template.Name + ".frx";
        var path = Path.Combine(_templatesDirectory, fileName);
        var content = _serializer.Serialize(template);
        await File.WriteAllTextAsync(path, content);
    }

    public async Task<ReportTemplate> LoadTemplateAsync(string fileName)
    {
        var path = Path.Combine(_templatesDirectory, fileName);
        var content = await File.ReadAllTextAsync(path);
        return _serializer.Deserialize(content);
    }

    public List<string> GetTemplateList()
    {
        return Directory.GetFiles(_templatesDirectory, "*.frx")
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .Cast<string>()
            .ToList();
    }
}

using ReportDesigner.Core.Models;

namespace ReportDesigner.Core.Interfaces;

public interface IReportTemplate
{
    string Id { get; }
    string Name { get; set; }
    string Description { get; set; }
    PageSettings Page { get; set; }
    List<Band> Bands { get; set; }
    List<DataSource> DataSources { get; set; }
    List<ReportParameter> Parameters { get; set; }
    string Script { get; set; }

    void Validate();
    byte[] Serialize();
    void Deserialize(byte[] data);
    ReportTemplate Clone();
    void Merge(ReportTemplate other);
}

public interface ITemplateStorage
{
    Task SaveAsync(ReportTemplate template, string path);
    Task<ReportTemplate> LoadAsync(string path);
    Task<List<ReportTemplate>> ListAsync(string directory);
    Task DeleteAsync(string path);
    Task<bool> ExistsAsync(string path);
}

namespace ReportDesigner.Core.Models;

public class ReportParameter
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = "System.String";
    public object DefaultValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Required { get; set; }
    public bool AllowNull { get; set; } = true;
    public string Expression { get; set; } = string.Empty; // For calculated parameters
    public List<string> AllowedValues { get; set; } = new(); // Dropdown values
    public string LookupDataSource { get; set; } = string.Empty; // For lookup parameters
    public string LookupDisplayField { get; set; } = string.Empty;
    public string LookupValueField { get; set; } = string.Empty;
    public bool MultiValue { get; set; } // Allow multiple selections
    public bool Hidden { get; set; } // Internal parameter
}

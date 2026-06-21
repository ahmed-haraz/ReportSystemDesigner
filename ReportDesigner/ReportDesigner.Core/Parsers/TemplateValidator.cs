using ReportDesigner.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace ReportDesigner.Core.Parsers;

public class TemplateValidator
{
    public ValidationResult Validate(ReportTemplate template)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(template.Name))
            errors.Add("Report name is required");

        if (template.Page.Width <= 0)
            errors.Add("Page width must be greater than 0");
        if (template.Page.Height <= 0)
            errors.Add("Page height must be greater than 0");
        if (template.Page.LeftMargin + template.Page.RightMargin >= template.Page.Width)
            errors.Add("Margins exceed page width");
        if (template.Page.TopMargin + template.Page.BottomMargin >= template.Page.Height)
            errors.Add("Margins exceed page height");

        if (!template.Bands.Any())
            errors.Add("At least one band is required");

        // Check for duplicate object names
        var allObjects = template.Bands.SelectMany(b => b.Objects).ToList();
        var duplicateNames = allObjects.GroupBy(o => o.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateNames.Any())
            errors.Add($"Duplicate object names: {string.Join(", ", duplicateNames)}");

        // Validate data source references
        foreach (var band in template.Bands.Where(b => !string.IsNullOrEmpty(b.DataSourceName)))
        {
            if (!template.DataSources.Any(ds => ds.Name == band.DataSourceName))
                errors.Add($"Band '{band.Name}' references unknown data source '{band.DataSourceName}'");
        }

        // Validate object data bindings
        foreach (var obj in allObjects.Where(o => !string.IsNullOrEmpty(o.DataBinding)))
        {
            var parts = obj.DataBinding.Split('.');
            if (parts.Length >= 2)
            {
                var dsName = parts[0];
                if (!template.DataSources.Any(ds => ds.Name == dsName))
                    errors.Add($"Object '{obj.Name}' references unknown data source '{dsName}'");
            }
        }

        // Check for overlapping objects (basic collision detection)
        foreach (var band in template.Bands)
        {
            var bandObjects = band.Objects.ToList();
            for (int i = 0; i < bandObjects.Count; i++)
            {
                for (int j = i + 1; j < bandObjects.Count; j++)
                {
                    var obj1 = bandObjects[i];
                    var obj2 = bandObjects[j];
                    if (RectanglesOverlap(obj1, obj2))
                    {
                        errors.Add($"Objects '{obj1.Name}' and '{obj2.Name}' overlap in band '{band.Name}'");
                    }
                }
            }
        }

        if (errors.Any())
            return new ValidationResult(string.Join("; ", errors));

        return ValidationResult.Success!;
    }

    private bool RectanglesOverlap(ReportObject a, ReportObject b)
    {
        return a.Left < b.Left + b.Width &&
               a.Left + a.Width > b.Left &&
               a.Top < b.Top + b.Height &&
               a.Top + a.Height > b.Top;
    }
}

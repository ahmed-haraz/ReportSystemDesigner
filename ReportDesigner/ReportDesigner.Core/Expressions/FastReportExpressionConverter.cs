using System.Text.RegularExpressions;

namespace ReportDesigner.Core.Expressions;

public class FastReportExpressionConverter
{
    /// <summary>
    /// Converts FastReport expression syntax to our internal Scriban format
    /// </summary>
    public static string ConvertFastReportToScriban(string fastReportExpression)
    {
        if (string.IsNullOrWhiteSpace(fastReportExpression))
            return string.Empty;

        var result = fastReportExpression;

        // System variables
        result = result.Replace("[Date]", "{{ now | date.to_string \"%Y-%m-%d\" }}");
        result = result.Replace("[Time]", "{{ now | date.to_string \"%H:%M:%S\" }}");
        result = result.Replace("[DateTime]", "{{ now | date.to_string \"%Y-%m-%d %H:%M:%S\" }}");
        result = result.Replace("[Page#]", "{{ page_number }}");
        result = result.Replace("[TotalPages#]", "{{ total_pages }}");
        result = result.Replace("[PageN]", "{{ page_number }}");
        result = result.Replace("[TotalPages]", "{{ total_pages }}");
        result = result.Replace("[Row#]", "{{ row_number }}");
        result = result.Replace("[ReportName]", "{{ report_name }}");
        result = result.Replace("[ReportDescription]", "{{ report_description }}");
        result = result.Replace("[Line#]", "{{ line_number }}");

        // Sum
        result = Regex.Replace(
            result,
            @"\[Sum\(([^,\)]+)(?:,\s*([^\)]+))?\)\]",
            m =>
            {
                var fieldRef = m.Groups[1].Value.Trim();
                var groupName = m.Groups[2].Success ? m.Groups[2].Value.Trim() : null;
                var parts = fieldRef.Split('.');

                if (parts.Length >= 2)
                {
                    var ds = parts[0].ToLowerInvariant();
                    var field = parts[1].ToLowerInvariant();

                    if (groupName != null)
                        return $@"{{{{ {ds} | array.where ""{groupName.ToLowerInvariant()}"" == group_value | array.map ""{field}"" | math.sum }}}}";

                    return $@"{{{{ {ds} | array.map ""{field}"" | math.sum }}}}";
                }

                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // Count
        result = Regex.Replace(
            result,
            @"\[Count\((?:Distinct\s+)?([^\)]+)\)\]",
            m =>
            {
                var fieldRef = m.Groups[1].Value.Trim();
                var parts = fieldRef.Split('.');

                if (parts.Length >= 1)
                {
                    var ds = parts[0].ToLowerInvariant();
                    return $"{{{{ {ds} | size }}}}";
                }

                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // Avg
        result = Regex.Replace(
            result,
            @"\[Avg\(([^\)]+)\)\]",
            m =>
            {
                var parts = m.Groups[1].Value.Split('.');
                if (parts.Length >= 2)
                    return $@"{{{{ {parts[0].ToLowerInvariant()} | array.map ""{parts[1].ToLowerInvariant()}"" | math.avg }}}}";
                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // Min
        result = Regex.Replace(
            result,
            @"\[Min\(([^\)]+)\)\]",
            m =>
            {
                var parts = m.Groups[1].Value.Split('.');
                if (parts.Length >= 2)
                    return $@"{{{{ {parts[0].ToLowerInvariant()} | array.map ""{parts[1].ToLowerInvariant()}"" | math.min }}}}";
                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // Max
        result = Regex.Replace(
            result,
            @"\[Max\(([^\)]+)\)\]",
            m =>
            {
                var parts = m.Groups[1].Value.Split('.');
                if (parts.Length >= 2)
                    return $@"{{{{ {parts[0].ToLowerInvariant()} | array.map ""{parts[1].ToLowerInvariant()}"" | math.max }}}}";
                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // First
        result = Regex.Replace(
            result,
            @"\[First\(([^\)]+)\)\]",
            m =>
            {
                var parts = m.Groups[1].Value.Split('.');
                if (parts.Length >= 2)
                    return $"{{{{ {parts[0].ToLowerInvariant()}.first.{parts[1].ToLowerInvariant()} }}}}";
                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // Last
        result = Regex.Replace(
            result,
            @"\[Last\(([^\)]+)\)\]",
            m =>
            {
                var parts = m.Groups[1].Value.Split('.');
                if (parts.Length >= 2)
                    return $"{{{{ {parts[0].ToLowerInvariant()}.last.{parts[1].ToLowerInvariant()} }}}}";
                return m.Value;
            },
            RegexOptions.IgnoreCase);

        // Field references
        result = Regex.Replace(
            result,
            @"\[([^,\[\]]+)(?:,\s*""([^""]+)"")?\]",
            m =>
            {
                var fieldRef = m.Groups[1].Value.Trim();
                var format = m.Groups[2].Success ? m.Groups[2].Value : null;

                if (fieldRef.Contains("("))
                    return m.Value;

                if (fieldRef.Contains("Date") ||
                    fieldRef.Contains("Page") ||
                    fieldRef.Contains("Row") ||
                    fieldRef.Contains("Report"))
                    return m.Value;

                var parts = fieldRef.Split('.');

                if (parts.Length >= 2)
                {
                    var ds = parts[0].ToLowerInvariant().Replace(" ", "_");
                    var field = parts[1].ToLowerInvariant().Replace(" ", "_");

                    if (format != null)
                        return $@"{{{{ {ds}.{field} | format ""{format}"" }}}}";

                    return $"{{{{ {ds}.{field} }}}}";
                }

                var singleField = fieldRef.ToLowerInvariant().Replace(" ", "_");

                if (format != null)
                    return $@"{{{{ {singleField} | format ""{format}"" }}}}";

                return $"{{{{ {singleField} }}}}";
            });

        return result;
    }

    /// <summary>
    /// Converts Scriban back to FastReport-like syntax
    /// </summary>
    public static string ConvertScribanToFastReport(string scribanExpression)
    {
        if (string.IsNullOrWhiteSpace(scribanExpression))
            return string.Empty;

        var result = scribanExpression;

        result = result.Replace("{{ now | date.to_string \"%Y-%m-%d\" }}", "[Date]");
        result = result.Replace("{{ now | date.to_string \"%H:%M:%S\" }}", "[Time]");
        result = result.Replace("{{ page_number }}", "[Page#]");
        result = result.Replace("{{ total_pages }}", "[TotalPages#]");
        result = result.Replace("{{ row_number }}", "[Row#]");

        result = Regex.Replace(
            result,
            @"\{\{\s*([a-z_][a-z0-9_]*)\.([a-z_][a-z0-9_]*)\s*\}\}",
            m =>
            {
                var ds = m.Groups[1].Value;
                var field = m.Groups[2].Value;
                return $"[{Capitalize(ds)}.{Capitalize(field)}]";
            });

        return result;
    }

    private static string Capitalize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
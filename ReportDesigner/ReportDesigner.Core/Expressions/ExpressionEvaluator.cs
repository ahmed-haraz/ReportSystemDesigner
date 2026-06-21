using ReportDesigner.Core.Interfaces;
using ReportDesigner.Core.Models;
using Scriban;
using Scriban.Runtime;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DataColumn = System.Data.DataColumn;

namespace ReportDesigner.Core.Expressions;

public class ExpressionEvaluator : IExpressionEngine
{
    private readonly ScriptObject _globalVariables = new();
    private readonly Dictionary<string, Delegate> _functions = new();

    public ExpressionEvaluator()
    {
        RegisterBuiltInFunctions();
    }

    private void RegisterBuiltInFunctions()
    {
        RegisterFunction("Now", (Func<DateTime>)(() => DateTime.Now));
        RegisterFunction("Today", (Func<DateTime>)(() => DateTime.Today));
        RegisterFunction("FormatDate", (Func<DateTime, string, string>)((date, format) => date.ToString(format)));

        RegisterFunction("Upper", (Func<string, string>)(s => s?.ToUpper() ?? string.Empty));
        RegisterFunction("Lower", (Func<string, string>)(s => s?.ToLower() ?? string.Empty));
        RegisterFunction("Trim", (Func<string, string>)(s => s?.Trim() ?? string.Empty));
        RegisterFunction("Substring", (Func<string, int, int, string>)((s, start, len) => s?.Substring(start, len) ?? string.Empty));
        RegisterFunction("Replace", (Func<string, string, string, string>)((s, old, @new) => s?.Replace(old, @new) ?? string.Empty));
        RegisterFunction("Contains", (Func<string, string, bool>)((s, search) => s?.Contains(search) ?? false));

        RegisterFunction("Round", (Func<double, int, double>)((val, digits) => Math.Round(val, digits)));
        RegisterFunction("Floor", (Func<double, double>)(Math.Floor));
        RegisterFunction("Ceiling", (Func<double, double>)(Math.Ceiling));
        RegisterFunction("Abs", (Func<double, double>)(Math.Abs));
        RegisterFunction("Min", (Func<double, double, double>)Math.Min);
        RegisterFunction("Max", (Func<double, double, double>)Math.Max);

        RegisterFunction("ToString", (Func<object, string>)(o => o?.ToString() ?? string.Empty));
        RegisterFunction("ToInt", (Func<object, int>)(Convert.ToInt32));
        RegisterFunction("ToDouble", (Func<object, double>)(Convert.ToDouble));
        RegisterFunction("ToDecimal", (Func<object, decimal>)(Convert.ToDecimal));
        RegisterFunction("ToDateTime", (Func<object, DateTime>)(Convert.ToDateTime));
        RegisterFunction("ToBool", (Func<object, bool>)(Convert.ToBoolean));

        RegisterFunction("IIf", (Func<bool, object, object, object>)((c, t, f) => c ? t : f));
        RegisterFunction("IsNull", (Func<object, bool>)(o => o == null || o == DBNull.Value));
        RegisterFunction("IsNullOrEmpty", (Func<string, bool>)(string.IsNullOrEmpty));
        RegisterFunction("IsNullOrWhiteSpace", (Func<string, bool>)(string.IsNullOrWhiteSpace));
    }

    public void RegisterDataSource(string name, DataTable data)
    {
        var list = new ScriptArray();

        foreach (DataRow row in data.Rows)
        {
            var obj = new ScriptObject();

            foreach (DataColumn col in data.Columns)
            {
                obj[col.ColumnName.ToLowerInvariant()] = row[col] ?? string.Empty;
            }

            list.Add(obj);
        }

        _globalVariables[name.ToLowerInvariant()] = list;
        _globalVariables[$"{name}_count".ToLowerInvariant()] = data.Rows.Count;
        _globalVariables[$"{name}_isempty".ToLowerInvariant()] = data.Rows.Count == 0;
    }

    public void RegisterDataSource<T>(string name, IEnumerable<T> data)
    {
        var list = new ScriptArray();
        var index = 0;

        foreach (var item in data)
        {
            var obj = new ScriptObject();
            var props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                obj[prop.Name.ToLowerInvariant()] = prop.GetValue(item) ?? string.Empty;
            }

            obj["__index"] = index++;
            list.Add(obj);
        }

        _globalVariables[name.ToLowerInvariant()] = list;
        _globalVariables[$"{name}_count".ToLowerInvariant()] = list.Count;
        _globalVariables[$"{name}_isempty".ToLowerInvariant()] = list.Count == 0;
    }

    public void RegisterParameter(string name, object value)
        => _globalVariables[name.ToLowerInvariant()] = value ?? string.Empty;

    public void RegisterVariable(string name, object value)
        => _globalVariables[name.ToLowerInvariant()] = value ?? string.Empty;

    public void RegisterFunction(string name, Delegate function)
    {
        _functions[name] = function;
        _globalVariables.Import(name, function);
    }

    public object Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return string.Empty;

        try
        {
            var scribanExpr = ConvertToScriban(expression);

            var template = Template.Parse(scribanExpr);

            var context = new TemplateContext
            {
                StrictVariables = false,
            };

            context.PushCulture(CultureInfo.InvariantCulture);
            context.PushGlobal(_globalVariables);

            foreach (var f in _functions)
                context.BuiltinObject.SetValue(f.Key, f.Value, false);

            return template.Evaluate(context) ?? string.Empty;
        }
        catch (Exception ex)
        {
            return $"[Error: {ex.Message}]";
        }
    }

    public string EvaluateToString(string expression)
        => Evaluate(expression)?.ToString() ?? string.Empty;

    public T Evaluate<T>(string expression)
    {
        var result = Evaluate(expression);
        if (result == null) return default!;

        try
        {
            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch
        {
            return default!;
        }
    }

    public bool EvaluateToBool(string expression)
    {
        var result = Evaluate(expression);
        if (result == null) return false;

        if (result is bool b) return b;
        if (result is string s)
            return !string.IsNullOrWhiteSpace(s) && s != "0" && s.ToLower() != "false";

        return Convert.ToBoolean(result);
    }

    public Expression Parse(string expression)
    {
        var expr = new Expression
        {
            RawExpression = expression,
            Dependencies = GetDependencies(expression)
        };

        var match = Regex.Match(expression,
            @"\[(Sum|Count|Average|Min|Max|First|Last)\(([^)]+)\)\]",
            RegexOptions.IgnoreCase);

        if (match.Success)
        {
            expr.IsAggregate = true;
            expr.AggregateFunction = Enum.Parse<AggregateFunction>(match.Groups[1].Value, true);

            var parts = match.Groups[2].Value.Split('.');
            if (parts.Length >= 2)
            {
                expr.AggregateDataSource = parts[0];
                expr.AggregateField = parts[1];
            }
        }

        return expr;
    }

    public bool Validate(string expression, out string error)
    {
        error = string.Empty;

        try
        {
            Template.Parse(ConvertToScriban(expression));
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public List<string> GetDependencies(string expression)
    {
        var list = new List<string>();
        var matches = Regex.Matches(expression, @"\[([^\]]+)\]");

        foreach (Match m in matches)
        {
            var v = m.Groups[1].Value;
            if (!v.Contains("(") && !v.Contains(" "))
                list.Add(v);
        }

        return list.Distinct().ToList();
    }

    public void Clear()
    {
        _globalVariables.Clear();
        RegisterBuiltInFunctions();
    }

    public void Reset() => Clear();

    private string ConvertToScriban(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return string.Empty;

        var result = expression;

        result = Regex.Replace(result,
            @"\[Sum\(([^)]+)\)\]",
            m =>
            {
                var p = m.Groups[1].Value.Split('.');
                return p.Length >= 2
                    ? $"{{{{ {p[0].ToLowerInvariant()} | array.map \"{p[1].ToLowerInvariant()}\" | math.sum }}}}"
                    : m.Value;
            },
            RegexOptions.IgnoreCase);

        result = Regex.Replace(result,
            @"\[Count\(([^)]+)\)\]",
            m =>
            {
                var p = m.Groups[1].Value.Split('.');
                return p.Length >= 1
                    ? $"{{{{ {p[0].ToLowerInvariant()} | size }}}}"
                    : m.Value;
            },
            RegexOptions.IgnoreCase);

        result = Regex.Replace(result,
            @"\[Average\(([^)]+)\)\]",
            m =>
            {
                var p = m.Groups[1].Value.Split('.');
                return p.Length >= 2
                    ? $"{{{{ {p[0].ToLowerInvariant()} | array.map \"{p[1].ToLowerInvariant()}\" | math.avg }}}}"
                    : m.Value;
            },
            RegexOptions.IgnoreCase);

        result = Regex.Replace(result,
            @"\[Min\(([^)]+)\)\]",
            m =>
            {
                var p = m.Groups[1].Value.Split('.');
                return p.Length >= 2
                    ? $"{{{{ {p[0].ToLowerInvariant()} | array.map \"{p[1].ToLowerInvariant()}\" | math.min }}}}"
                    : m.Value;
            },
            RegexOptions.IgnoreCase);

        result = Regex.Replace(result,
            @"\[Max\(([^)]+)\)\]",
            m =>
            {
                var p = m.Groups[1].Value.Split('.');
                return p.Length >= 2
                    ? $"{{{{ {p[0].ToLowerInvariant()} | array.map \"{p[1].ToLowerInvariant()}\" | math.max }}}}"
                    : m.Value;
            },
            RegexOptions.IgnoreCase);

        result = Regex.Replace(result,
            @"\[([^\[\]\(\)]+)\]",
            m =>
            {
                var f = m.Groups[1].Value;

                if (f.Contains("(")) return m.Value;

                var p = f.Split('.');
                if (p.Length >= 2)
                    return $"{{{{ {p[0].ToLowerInvariant()}.{p[1].ToLowerInvariant()} }}}}";

                return $"{{{{ {f.ToLowerInvariant()} }}}}";
            });

        return result;
    }
}
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

namespace ReportDesigner.Core.Expressions;

public class ScriptEngine
{
    private readonly List<string> _references = new();
    private readonly StringBuilder _scriptBuilder = new();
    private CompilerResults? _compiledAssembly;

    public ScriptEngine()
    {
        // Add default references
        _references.Add("System.dll");
        _references.Add("System.Core.dll");
        _references.Add("System.Data.dll");
        _references.Add("System.Drawing.dll");
        _references.Add("System.Windows.Forms.dll");
    }

    public void AddReference(string assemblyPath)
    {
        if (!_references.Contains(assemblyPath))
            _references.Add(assemblyPath);
    }

    public void AddScript(string script)
    {
        _scriptBuilder.AppendLine(script);
    }

    public bool Compile(out string errors)
    {
        errors = string.Empty;

        var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false,
            IncludeDebugInformation = false
        };

        foreach (var reference in _references)
        {
            parameters.ReferencedAssemblies.Add(reference);
        }

        var fullScript = $@"
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

public class ReportScript 
{{
    public Dictionary<string, object> Variables {{ get; set; }} = new();
    public Dictionary<string, DataTable> DataSources {{ get; set; }} = new();

    {_scriptBuilder}
}}
";

        _compiledAssembly = provider.CompileAssemblyFromSource(parameters, fullScript);

        if (_compiledAssembly.Errors.HasErrors)
        {
            var errorList = new List<string>();
            foreach (CompilerError error in _compiledAssembly.Errors)
            {
                if (!error.IsWarning)
                    errorList.Add($"Line {error.Line}: {error.ErrorText}");
            }
            errors = string.Join("\n", errorList);
            return false;
        }

        return true;
    }

    public object? ExecuteMethod(string methodName, params object[] parameters)
    {
        if (_compiledAssembly == null)
            throw new InvalidOperationException("Script must be compiled before execution");

        var type = _compiledAssembly.CompiledAssembly.GetType("ReportScript");
        if (type == null) return null;

        var instance = Activator.CreateInstance(type);
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

        if (method == null) return null;

        return method.Invoke(instance, parameters);
    }

    public void SetVariable(string name, object value)
    {
        if (_compiledAssembly == null) return;

        var type = _compiledAssembly.CompiledAssembly.GetType("ReportScript");
        if (type == null) return;

        var instance = Activator.CreateInstance(type);
        var prop = type.GetProperty("Variables");
        if (prop?.GetValue(instance) is Dictionary<string, object> vars)
        {
            vars[name] = value;
        }
    }

    public void Reset()
    {
        _scriptBuilder.Clear();
        _compiledAssembly = null;
    }
}

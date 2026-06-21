using ReportDesigner.Core.Models;
using System.Data;

namespace ReportDesigner.Core.Interfaces;

public interface IExpressionEngine
{
    void RegisterDataSource(string name, DataTable data);
    void RegisterDataSource<T>(string name, IEnumerable<T> data);
    void RegisterParameter(string name, object value);
    void RegisterVariable(string name, object value);
    void RegisterFunction(string name, Delegate function);

    object Evaluate(string expression);
    string EvaluateToString(string expression);
    T Evaluate<T>(string expression);
    bool EvaluateToBool(string expression);

    Expression Parse(string expression);
    bool Validate(string expression, out string error);
    List<string> GetDependencies(string expression);

    void Clear();
    void Reset();
}

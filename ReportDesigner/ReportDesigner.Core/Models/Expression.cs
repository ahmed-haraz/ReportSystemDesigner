namespace ReportDesigner.Core.Models;

public class Expression
{
    public string RawExpression { get; set; } = string.Empty;
    public string CompiledExpression { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new(); // Data fields used
    public bool IsAggregate { get; set; }
    public AggregateFunction AggregateFunction { get; set; } = AggregateFunction.None;
    public string AggregateDataSource { get; set; } = string.Empty;
    public string AggregateField { get; set; } = string.Empty;
    public string AggregateGroup { get; set; } = string.Empty; // Group name for aggregate
    public bool IsCalculated { get; set; }
    public string DataType { get; set; } = "System.String";
    public string FormatString { get; set; } = string.Empty;
    public bool SuppressIfDuplicate { get; set; }
    public bool ResetPageNumber { get; set; } // Reset page number on group change
    public bool RunningTotal { get; set; } // Accumulate across rows
    public string Condition { get; set; } = string.Empty; // Only calculate if condition met
}

public class ExpressionResult
{
    public object Value { get; set; }
    public string FormattedValue { get; set; } = string.Empty;
    public bool IsNull { get; set; }
    public string DataType { get; set; } = "System.String";
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

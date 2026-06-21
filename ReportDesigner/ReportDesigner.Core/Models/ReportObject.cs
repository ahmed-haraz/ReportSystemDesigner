namespace ReportDesigner.Core.Models;

public class ReportObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public ObjectType Type { get; set; } = ObjectType.Text;
    public string Name { get; set; } = string.Empty;
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; } = 100;
    public float Height { get; set; } = 20;
    public string DataBinding { get; set; } = string.Empty;
    public string Expression { get; set; } = string.Empty; 
    public string Text { get; set; } = string.Empty; 
    public TextProperties TextProps { get; set; } = new();
    public BorderProperties Border { get; set; } = new();
    public FillProperties Fill { get; set; } = new();
    public string FormatString { get; set; } = string.Empty;
    public bool CanGrow { get; set; } = true;
    public bool CanShrink { get; set; }
    public bool Visible { get; set; } = true;
    public string VisibleExpression { get; set; } = string.Empty;
    public string Hyperlink { get; set; } = string.Empty;
    public string Bookmark { get; set; } = string.Empty;
    public string PrintOn { get; set; } = "AllPages";
    public int ZIndex { get; set; } = 0;
    public string Cursor { get; set; } = "Default";
    public string Tag { get; set; } = string.Empty; 
    public PictureProperties PictureProps { get; set; }
    public BarcodeProperties BarcodeProps { get; set; }
    public ChartProperties ChartProps { get; set; }
    public TableProperties TableProps { get; set; }
    public ShapeProperties ShapeProps { get; set; }
    public SubreportProperties SubreportProps { get; set; }
}

public class PictureProperties
{
    public string ImageSource { get; set; } = string.Empty; 
    public string ImageFormat { get; set; } = "png"; 
    public bool KeepAspectRatio { get; set; } = true;
    public string Alignment { get; set; } = "Center";
    public int Rotation { get; set; } = 0;
    public string GrayscaleExpression { get; set; } = string.Empty;
    public string TransparencyExpression { get; set; } = string.Empty;
}

public class BarcodeProperties
{
    public BarcodeType BarcodeType { get; set; } = BarcodeType.Code128;
    public string Data { get; set; } = string.Empty;
    public bool ShowText { get; set; } = true;
    public float BarWidth { get; set; } = 2;
    public float BarHeight { get; set; } = 50;
    public string TextPosition { get; set; } = "Bottom";
    public bool Checksum { get; set; } = true;
    public string ErrorCorrection { get; set; } = "M"; 
}

public class ChartProperties
{
    public ChartType ChartType { get; set; } = ChartType.Bar;
    public string Title { get; set; } = string.Empty;
    public string XAxisField { get; set; } = string.Empty;
    public string YAxisField { get; set; } = string.Empty;
    public string SeriesField { get; set; } = string.Empty;
    public string ValueField { get; set; } = string.Empty;
    public bool ShowLegend { get; set; } = true;
    public bool ShowValues { get; set; } = true;
    public string ColorPalette { get; set; } = "Default";
    public int Width { get; set; } = 400;
    public int Height { get; set; } = 300;
}

public class TableProperties
{
    public int RowCount { get; set; } = 1;
    public int ColumnCount { get; set; } = 1;
    public List<TableRow> Rows { get; set; } = new();
    public List<TableColumn> Columns { get; set; } = new();
    public bool RepeatHeaderRow { get; set; } = true;
    public string DataSourceName { get; set; } = string.Empty;
    public bool AutoWidth { get; set; } = true;
}

public class TableRow
{
    public float Height { get; set; } = 20;
    public bool IsHeader { get; set; }
    public bool IsFooter { get; set; }
    public List<TableCell> Cells { get; set; } = new();
}

public class TableColumn
{
    public float Width { get; set; } = 100;
    public bool AutoSize { get; set; } = true;
    public string FieldName { get; set; } = string.Empty;
}

public class TableCell : ReportObject
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public int RowSpan { get; set; } = 1;
    public int ColumnSpan { get; set; } = 1;
    public string Aggregate { get; set; } = string.Empty; // Sum, Count, etc.
}

public class ShapeProperties
{
    public ShapeType ShapeType { get; set; } = ShapeType.Rectangle;
    public int CornerRadius { get; set; } = 0;
    public string StartArrow { get; set; } = "None";
    public string EndArrow { get; set; } = "None";
    public int ArrowSize { get; set; } = 5;
    public bool FillShape { get; set; } = true;
}

public class SubreportProperties
{
    public string ReportFile { get; set; } = string.Empty;
    public string ParameterExpression { get; set; } = string.Empty;
    public bool PrintOnParent { get; set; } = true;
    public bool KeepTogether { get; set; } = true;
}

using System;

namespace ReportDesigner.Core.Models;

public enum BandType
{
    ReportTitle,
    ReportSummary,
    PageHeader,
    PageFooter,
    ColumnHeader,
    ColumnFooter,
    DataHeader,
    Data,
    DataFooter,
    GroupHeader,
    GroupFooter,
    Child,
    Overlay
}

public enum ObjectType
{
    Text,
    Picture,
    Line,
    Shape,
    Barcode,
    Table,
    Matrix,
    Checkbox,
    Chart,
    Subreport,
    RichText,
    Html
}

public enum DataSourceType
{
    SqlServer,
    SQLite,
    PostgreSQL,
    MySQL,
    Oracle,
    MongoDB,
    Json,
    Xml,
    Csv,
    Excel,
    BusinessObject,
    OData,
    WebService
}

public enum PageOrientation
{
    Portrait,
    Landscape
}

public enum TextAlignment
{
    Left,
    Center,
    Right,
    Justify
}

public enum BorderStyle
{
    Solid,
    Dashed,
    Dotted,
    Double,
    None
}

public enum FillType
{
    Solid,
    Gradient,
    Pattern,
    Image
}

public enum ShapeType
{
    Rectangle,
    RoundRectangle,
    Ellipse,
    Triangle,
    Diamond,
    Line,
    Arrow
}

public enum BarcodeType
{
    Code128,
    Code39,
    EAN13,
    EAN8,
    UPC_A,
    QRCode,
    DataMatrix,
    PDF417
}

public enum ChartType
{
    Bar,
    Line,
    Pie,
    Doughnut,
    Area,
    Scatter,
    Radar
}

public enum AggregateFunction
{
    Sum,
    Count,
    Average,
    Min,
    Max,
    First,
    Last,
    None
}

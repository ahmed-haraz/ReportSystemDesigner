# ReportSystem - FastReport-like Reporting for .NET MAUI

A complete reporting system with drag-and-drop designer, SQL/SQLite connectivity, and native MAUI integration.

## Features

- **Visual Report Designer** (WPF Desktop) - Drag & drop bands and objects
- **SQL/SQLite/PostgreSQL/MySQL** data source connectivity
- **FastReport-compatible .frx format** - Industry standard template format
- **Pure C# MAUI PDF rendering** - No external dependencies via QuestPDF
- **Native MAUI PDF viewing** - Share, print, preview on all platforms
- **Expression engine** - Aggregates, formatting, conditional logic
- **Subreports, charts, barcodes, tables** - Full feature set

## Architecture

```
ReportSystem/
├── ReportDesigner/           # Windows Desktop Designer (WPF)
│   ├── Core/                 # Shared models & parsers
│   ├── Desktop/              # WPF drag-drop designer
│   └── Web/                  # Optional Blazor designer
├── MauiReportEngine/         # Cross-platform MAUI engine
│   ├── Core/                 # Render models
│   ├── Renderer/             # QuestPDF implementation
│   └── MAUI/                 # MAUI integration services
└── Shared/Templates/         # .frx template files
```

## Quick Start

### 1. Install Dependencies

```bash
# Core packages
dotnet add package QuestPDF
dotnet add package Scriban
dotnet add package Microsoft.Data.Sqlite

# WPF Designer
dotnet add package CommunityToolkit.Mvvm

# MAUI (optional)
dotnet add package Syncfusion.Maui.PdfViewer  # or free alternatives
```

### 2. Use the Designer

1. Open `ReportDesigner.Desktop` in Visual Studio
2. Create new report or open existing .frx
3. Drag bands and objects from toolbox
4. Connect to SQLite/SQL Server database
5. Preview and save template

### 3. Generate Reports in MAUI

```csharp
// In your MAUI app
var reportService = new ReportService();

var data = new {
    SalesData = await GetSalesDataAsync(),
    ReportTitle = "Monthly Sales",
    CompanyName = "Acme Corp"
};

var pdfBytes = await reportService.GenerateReportAsync("SalesReport.frx", data);
await reportService.PreviewReportAsync(pdfBytes, "report.pdf");
```

## Template Format (.frx)

Templates use XML format compatible with FastReport:

```xml
<Report Name="Sales Report">
  <Dictionary>
    <TableDataSource Name="SalesData" DataType="SQLite" 
                     SelectCommand="SELECT * FROM Sales"/>
  </Dictionary>
  <ReportPage Width="595" Height="842">
    <ReportTitleBand Name="Title" Height="80">
      <TextObject Name="txtTitle" Left="0" Top="0" Width="515" Height="30"
                  Text="[ReportTitle]" Font="Arial, 16pt, Bold"/>
    </ReportTitleBand>
    <DataBand Name="Details" Height="20" DataSource="SalesData">
      <TextObject DataColumn="SalesData.ProductName" Left="0" Top="0" Width="200"/>
    </DataBand>
  </ReportPage>
</Report>
```

## Data Sources

Supported database types:
- SQLite (built-in)
- SQL Server
- PostgreSQL
- MySQL
- JSON/XML files
- Business objects

## Expression Syntax

| Expression | Description |
|-----------|-------------|
| `[FieldName]` | Data field reference |
| `[Sum(DataSource.Field)]` | Sum aggregate |
| `[Count(DataSource)]` | Row count |
| `[Date]` | Current date |
| `[Page#]` | Page number |
| `[TotalPages#]` | Total pages |

## License

MIT License - Free for commercial and personal use.

QuestPDF Community Edition is free for non-commercial use.
For commercial use, obtain a QuestPDF license or use alternative renderers.

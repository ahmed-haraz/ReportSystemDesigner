# Architecture Overview

## System Components

### 1. Report Designer (WPF Desktop)
- **Purpose**: Visual drag-and-drop report design
- **Technology**: WPF, .NET 8/9/10, CommunityToolkit.Mvvm
- **Output**: .frx XML template files
- **Features**:
  - Band-based layout (ReportTitle, PageHeader, Data, PageFooter, etc.)
  - Object toolbox (Text, Picture, Line, Shape, Barcode, Chart, Table)
  - Data source wizard (SQL, SQLite, PostgreSQL, MySQL)
  - Property panel with live editing
  - Expression builder with IntelliSense
  - Preview with sample data

### 2. Report Engine (MAUI Cross-Platform)
- **Purpose**: Render .frx templates to PDF on any platform
- **Technology**: .NET MAUI, QuestPDF, Scriban
- **Input**: .frx template + JSON data
- **Output**: PDF byte array
- **Features**:
  - Band rendering engine
  - Expression evaluation
  - Aggregate calculation (Sum, Count, Average, Min, Max)
  - Multi-page support
  - Watermarks, headers/footers
  - Subreports

### 3. Template Format (.frx)
- XML-based format compatible with FastReport
- Self-contained: layout, data sources, expressions, scripts
- Portable between designer and engine
- Version controlled (Git-friendly)

## Data Flow

```
[Database] → [Designer Data Source] → [Template (.frx)]
                                              ↓
[MAUI App] → [JSON Data] → [Report Engine] → [PDF] → [Viewer/Share/Print]
```

## Rendering Pipeline

1. **Parse**: Load .frx template into object model
2. **Bind**: Register data sources and parameters
3. **Evaluate**: Process expressions and aggregates
4. **Layout**: Calculate positions and page breaks
5. **Render**: Generate PDF via QuestPDF
6. **Output**: Return byte array or stream

## Platform Support

| Platform | Designer | Engine | Viewer |
|----------|----------|--------|--------|
| Windows | ✅ Full | ✅ Yes | ✅ Yes |
| macOS | ❌ No | ✅ Yes | ✅ Yes |
| iOS | ❌ No | ✅ Yes | ✅ Yes |
| Android | ❌ No | ✅ Yes | ✅ Yes |
| Linux | ❌ No | ✅ Yes | ✅ Yes |

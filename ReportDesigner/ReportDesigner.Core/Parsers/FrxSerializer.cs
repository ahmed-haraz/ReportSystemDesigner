using System.Xml.Linq;
using System.Globalization;
using ReportDesigner.Core.Models;

namespace ReportDesigner.Core.Parsers;

public class FrxSerializer
{
    private const string Version = "2026.1";

    public string Serialize(ReportTemplate template)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Report",
                new XAttribute("Version", Version),
                new XAttribute("Name", template.Name),
                new XAttribute("Description", template.Description),
                new XAttribute("Author", template.Author),
                new XAttribute("CreatedDate", template.CreatedDate.ToString("o")),
                new XAttribute("ModifiedDate", template.ModifiedDate.ToString("o")),
                new XAttribute("ScriptLanguage", template.ScriptLanguage),
                new XAttribute("ConvertNulls", template.ConvertNulls),
                new XAttribute("DoublePass", template.DoublePass),

                // Dictionary (Data Sources)
                new XElement("Dictionary",
                    template.DataSources.Select(ds => SerializeDataSource(ds)),
                    template.Parameters.Select(p => SerializeParameter(p))
                ),

                // Styles
                new XElement("Styles",
                    template.Styles.Select(s => new XElement("Style",
                        new XAttribute("Name", s.Key),
                        new XAttribute("Value", s.Value)
                    ))
                ),

                // Report Page
                SerializePageSettings(template.Page),

                // Script
                new XElement("Script", template.Script),

                // Referenced Assemblies
                new XElement("ReferencedAssemblies",
                    template.ReferencedAssemblies.Select(a => new XElement("Assembly", a))
                )
            )
        );

        return doc.ToString();
    }

    private XElement SerializeDataSource(DataSource ds)
    {
        var element = new XElement("TableDataSource",
            new XAttribute("Name", ds.Name),
            new XAttribute("DataType", ds.Type.ToString()),
            new XAttribute("Enabled", ds.Enabled),
            new XAttribute("CommandTimeout", ds.CommandTimeout)
        );

        if (!string.IsNullOrEmpty(ds.ConnectionString))
            element.Add(new XAttribute("ConnectionString", ds.ConnectionString));
        if (!string.IsNullOrEmpty(ds.SelectCommand))
            element.Add(new XAttribute("SelectCommand", ds.SelectCommand));
        if (!string.IsNullOrEmpty(ds.TableName))
            element.Add(new XAttribute("TableName", ds.TableName));
        if (!string.IsNullOrEmpty(ds.AssemblyName))
            element.Add(new XAttribute("AssemblyName", ds.AssemblyName));
        if (!string.IsNullOrEmpty(ds.ClassName))
            element.Add(new XAttribute("ClassName", ds.ClassName));
        if (!string.IsNullOrEmpty(ds.MethodName))
            element.Add(new XAttribute("MethodName", ds.MethodName));

        element.Add(ds.Columns.Select(c => SerializeColumn(c)));
        element.Add(ds.Parameters.Select(p => SerializeParameter(p)));

        return element;
    }

    private XElement SerializeColumn(DataColumn c)
    {
        var element = new XElement("Column",
            new XAttribute("Name", c.Name),
            new XAttribute("DataType", c.DataType)
        );
        if (!string.IsNullOrEmpty(c.Caption))
            element.Add(new XAttribute("Caption", c.Caption));
        if (c.Size > 0)
            element.Add(new XAttribute("Size", c.Size));
        element.Add(new XAttribute("Nullable", c.Nullable));
        element.Add(new XAttribute("PrimaryKey", c.PrimaryKey));
        element.Add(new XAttribute("ReadOnly", c.ReadOnly));
        if (!string.IsNullOrEmpty(c.Expression))
            element.Add(new XAttribute("Expression", c.Expression));
        if (!string.IsNullOrEmpty(c.Format))
            element.Add(new XAttribute("Format", c.Format));
        return element;
    }

    private XElement SerializeParameter(ReportParameter p)
    {
        var element = new XElement("Parameter",
            new XAttribute("Name", p.Name),
            new XAttribute("DataType", p.DataType),
            new XAttribute("Required", p.Required),
            new XAttribute("AllowNull", p.AllowNull),
            new XAttribute("Hidden", p.Hidden),
            new XAttribute("MultiValue", p.MultiValue)
        );
        if (p.DefaultValue != null)
            element.Add(new XAttribute("DefaultValue", p.DefaultValue.ToString()));
        if (!string.IsNullOrEmpty(p.Description))
            element.Add(new XAttribute("Description", p.Description));
        if (!string.IsNullOrEmpty(p.Expression))
            element.Add(new XAttribute("Expression", p.Expression));
        if (!string.IsNullOrEmpty(p.LookupDataSource))
            element.Add(new XAttribute("LookupDataSource", p.LookupDataSource));
        if (!string.IsNullOrEmpty(p.LookupDisplayField))
            element.Add(new XAttribute("LookupDisplayField", p.LookupDisplayField));
        if (!string.IsNullOrEmpty(p.LookupValueField))
            element.Add(new XAttribute("LookupValueField", p.LookupValueField));
        if (p.AllowedValues?.Any() == true)
            element.Add(new XElement("AllowedValues", p.AllowedValues.Select(v => new XElement("Value", v))));
        return element;
    }

    private XElement SerializePageSettings(PageSettings ps)
    {
        return new XElement("ReportPage",
            new XAttribute("Width", ps.Width),
            new XAttribute("Height", ps.Height),
            new XAttribute("LeftMargin", ps.LeftMargin),
            new XAttribute("RightMargin", ps.RightMargin),
            new XAttribute("TopMargin", ps.TopMargin),
            new XAttribute("BottomMargin", ps.BottomMargin),
            new XAttribute("Orientation", ps.Orientation.ToString()),
            new XAttribute("PaperName", ps.PaperName),
            new XAttribute("ColumnWidth", ps.ColumnWidth),
            new XAttribute("ColumnCount", ps.ColumnCount),
            new XAttribute("ColumnGap", ps.ColumnGap),
            new XAttribute("Resolution", ps.Resolution),

            // Watermark
            new XElement("Watermark",
                new XAttribute("Text", ps.WatermarkText),
                new XAttribute("Font", ps.WatermarkFont),
                new XAttribute("FontSize", ps.WatermarkFontSize),
                new XAttribute("Color", ps.WatermarkColor),
                new XAttribute("Opacity", ps.WatermarkOpacity),
                new XAttribute("ShowBehind", ps.WatermarkShowBehind)
            )
        );
    }

    private XElement SerializeBand(Band band)
    {
        var element = new XElement($"{band.Type}Band",
            new XAttribute("Name", band.Name),
            new XAttribute("Height", band.Height),
            new XAttribute("Top", band.Top),
            new XAttribute("CanGrow", band.CanGrow),
            new XAttribute("CanShrink", band.CanShrink),
            new XAttribute("RepeatOnEveryPage", band.RepeatOnEveryPage),
            new XAttribute("PrintOnBottom", band.PrintOnBottom),
            new XAttribute("StartNewPage", band.StartNewPage),
            new XAttribute("PrintIfDetailEmpty", band.PrintIfDetailEmpty),
            new XAttribute("KeepTogether", band.KeepTogether),
            new XAttribute("Outline", band.Outline),
            new XAttribute("OutlineLevel", band.OutlineLevel)
        );

        if (!string.IsNullOrEmpty(band.DataSourceName))
            element.Add(new XAttribute("DataSource", band.DataSourceName));
        if (!string.IsNullOrEmpty(band.Condition))
            element.Add(new XAttribute("Condition", band.Condition));
        if (!string.IsNullOrEmpty(band.FilterExpression))
            element.Add(new XAttribute("Filter", band.FilterExpression));
        if (!string.IsNullOrEmpty(band.SortExpression))
            element.Add(new XAttribute("Sort", band.SortExpression));
        if (!string.IsNullOrEmpty(band.GroupExpression))
            element.Add(new XAttribute("GroupExpression", band.GroupExpression));
        if (!string.IsNullOrEmpty(band.BeforePrintScript))
            element.Add(new XElement("BeforePrintScript", band.BeforePrintScript));
        if (!string.IsNullOrEmpty(band.AfterPrintScript))
            element.Add(new XElement("AfterPrintScript", band.AfterPrintScript));
        if (!string.IsNullOrEmpty(band.OutlineText))
            element.Add(new XAttribute("OutlineText", band.OutlineText));

        element.Add(band.Objects.Select(o => SerializeObject(o)));

        return element;
    }

    private XElement SerializeObject(ReportObject obj)
    {
        var element = new XElement($"{obj.Type}Object",
            new XAttribute("Name", obj.Name),
            new XAttribute("Left", obj.Left),
            new XAttribute("Top", obj.Top),
            new XAttribute("Width", obj.Width),
            new XAttribute("Height", obj.Height),
            new XAttribute("CanGrow", obj.CanGrow),
            new XAttribute("CanShrink", obj.CanShrink),
            new XAttribute("Visible", obj.Visible),
            new XAttribute("ZIndex", obj.ZIndex)
        );

        if (!string.IsNullOrEmpty(obj.DataBinding))
            element.Add(new XAttribute("DataColumn", obj.DataBinding));
        if (!string.IsNullOrEmpty(obj.Expression))
            element.Add(new XAttribute("Expression", obj.Expression));
        if (!string.IsNullOrEmpty(obj.Text))
            element.Add(new XAttribute("Text", obj.Text));
        if (!string.IsNullOrEmpty(obj.FormatString))
            element.Add(new XAttribute("Format", obj.FormatString));
        if (!string.IsNullOrEmpty(obj.VisibleExpression))
            element.Add(new XAttribute("VisibleExpression", obj.VisibleExpression));
        if (!string.IsNullOrEmpty(obj.Hyperlink))
            element.Add(new XAttribute("Hyperlink", obj.Hyperlink));
        if (!string.IsNullOrEmpty(obj.Bookmark))
            element.Add(new XAttribute("Bookmark", obj.Bookmark));
        if (!string.IsNullOrEmpty(obj.PrintOn) && obj.PrintOn != "AllPages")
            element.Add(new XAttribute("PrintOn", obj.PrintOn));
        if (!string.IsNullOrEmpty(obj.Tag))
            element.Add(new XAttribute("Tag", obj.Tag));

        // Text properties
        if (obj.TextProps != null)
            element.Add(SerializeTextProperties(obj.TextProps));

        // Border
        if (obj.Border?.AnyBorder == true)
            element.Add(SerializeBorderProperties(obj.Border));

        // Fill
        if (obj.Fill?.Color != "#FFFFFF" || obj.Fill?.Type != FillType.Solid)
            element.Add(SerializeFillProperties(obj.Fill));

        // Type-specific
        switch (obj.Type)
        {
            case ObjectType.Picture when obj.PictureProps != null:
                element.Add(SerializePictureProperties(obj.PictureProps));
                break;
            case ObjectType.Barcode when obj.BarcodeProps != null:
                element.Add(SerializeBarcodeProperties(obj.BarcodeProps));
                break;
            case ObjectType.Chart when obj.ChartProps != null:
                element.Add(SerializeChartProperties(obj.ChartProps));
                break;
            case ObjectType.Table when obj.TableProps != null:
                element.Add(SerializeTableProperties(obj.TableProps));
                break;
            case ObjectType.Shape when obj.ShapeProps != null:
                element.Add(SerializeShapeProperties(obj.ShapeProps));
                break;
            case ObjectType.Subreport when obj.SubreportProps != null:
                element.Add(SerializeSubreportProperties(obj.SubreportProps));
                break;
        }

        return element;
    }

    private XElement SerializeTextProperties(TextProperties tp)
    {
        return new XElement("TextProperties",
            new XAttribute("Font", $"{tp.FontName}, {tp.FontSize}pt"),
            new XAttribute("Bold", tp.Bold),
            new XAttribute("Italic", tp.Italic),
            new XAttribute("Underline", tp.Underline),
            new XAttribute("Strikeout", tp.Strikeout),
            new XAttribute("ForeColor", tp.ForeColor),
            new XAttribute("BackColor", tp.BackColor),
            new XAttribute("Alignment", tp.Alignment.ToString()),
            new XAttribute("WordWrap", tp.WordWrap),
            new XAttribute("AutoShrink", tp.AutoShrink),
            new XAttribute("AutoExpand", tp.AutoExpand),
            new XAttribute("Angle", tp.Angle),
            new XAttribute("Format", tp.FormatString),
            new XAttribute("RightToLeft", tp.RightToLeft),
            new XAttribute("CharSpacing", tp.CharSpacing),
            new XAttribute("LineSpacing", tp.LineSpacing),
            new XAttribute("TrimWhitespace", tp.TrimWhitespace),
            new XAttribute("HtmlTags", tp.HtmlTags)
        );
    }

    private XElement SerializeBorderProperties(BorderProperties bp)
    {
        return new XElement("Border",
            new XAttribute("Left", bp.Left),
            new XAttribute("Right", bp.Right),
            new XAttribute("Top", bp.Top),
            new XAttribute("Bottom", bp.Bottom),
            new XAttribute("Width", bp.Width),
            new XAttribute("Color", bp.Color),
            new XAttribute("Style", bp.Style.ToString()),
            new XAttribute("Radius", bp.Radius)
        );
    }

    private XElement SerializeFillProperties(FillProperties fp)
    {
        var element = new XElement("Fill",
            new XAttribute("Color", fp.Color),
            new XAttribute("Type", fp.Type.ToString())
        );
        if (fp.Type == FillType.Gradient)
        {
            element.Add(new XAttribute("GradientStart", fp.GradientStartColor));
            element.Add(new XAttribute("GradientEnd", fp.GradientEndColor));
            element.Add(new XAttribute("GradientAngle", fp.GradientAngle));
        }
        if (fp.Type == FillType.Pattern)
        {
            element.Add(new XAttribute("PatternColor", fp.PatternColor));
            element.Add(new XAttribute("PatternStyle", fp.PatternStyle));
        }
        if (fp.Type == FillType.Image)
        {
            element.Add(new XAttribute("ImageSource", fp.ImageSource));
            element.Add(new XAttribute("ImageTile", fp.ImageTile));
            element.Add(new XAttribute("ImageAlignment", fp.ImageAlignment));
        }
        return element;
    }

    private XElement SerializePictureProperties(PictureProperties pp)
    {
        return new XElement("PictureProperties",
            new XAttribute("ImageSource", pp.ImageSource),
            new XAttribute("ImageFormat", pp.ImageFormat),
            new XAttribute("KeepAspectRatio", pp.KeepAspectRatio),
            new XAttribute("Alignment", pp.Alignment),
            new XAttribute("Rotation", pp.Rotation)
        );
    }

    private XElement SerializeBarcodeProperties(BarcodeProperties bp)
    {
        return new XElement("BarcodeProperties",
            new XAttribute("BarcodeType", bp.BarcodeType.ToString()),
            new XAttribute("Data", bp.Data),
            new XAttribute("ShowText", bp.ShowText),
            new XAttribute("BarWidth", bp.BarWidth),
            new XAttribute("BarHeight", bp.BarHeight),
            new XAttribute("TextPosition", bp.TextPosition),
            new XAttribute("Checksum", bp.Checksum),
            new XAttribute("ErrorCorrection", bp.ErrorCorrection)
        );
    }

    private XElement SerializeChartProperties(ChartProperties cp)
    {
        return new XElement("ChartProperties",
            new XAttribute("ChartType", cp.ChartType.ToString()),
            new XAttribute("Title", cp.Title),
            new XAttribute("XAxisField", cp.XAxisField),
            new XAttribute("YAxisField", cp.YAxisField),
            new XAttribute("SeriesField", cp.SeriesField),
            new XAttribute("ValueField", cp.ValueField),
            new XAttribute("ShowLegend", cp.ShowLegend),
            new XAttribute("ShowValues", cp.ShowValues),
            new XAttribute("ColorPalette", cp.ColorPalette),
            new XAttribute("Width", cp.Width),
            new XAttribute("Height", cp.Height)
        );
    }

    private XElement SerializeTableProperties(TableProperties tp)
    {
        return new XElement("TableProperties",
            new XAttribute("RowCount", tp.RowCount),
            new XAttribute("ColumnCount", tp.ColumnCount),
            new XAttribute("RepeatHeaderRow", tp.RepeatHeaderRow),
            new XAttribute("DataSource", tp.DataSourceName),
            new XAttribute("AutoWidth", tp.AutoWidth),
            tp.Rows.Select(r => new XElement("Row",
                new XAttribute("Height", r.Height),
                new XAttribute("IsHeader", r.IsHeader),
                new XAttribute("IsFooter", r.IsFooter),
                r.Cells.Select(c => SerializeObject(c))
            )),
            tp.Columns.Select(c => new XElement("Column",
                new XAttribute("Width", c.Width),
                new XAttribute("AutoSize", c.AutoSize),
                new XAttribute("FieldName", c.FieldName)
            ))
        );
    }

    private XElement SerializeShapeProperties(ShapeProperties sp)
    {
        return new XElement("ShapeProperties",
            new XAttribute("ShapeType", sp.ShapeType.ToString()),
            new XAttribute("CornerRadius", sp.CornerRadius),
            new XAttribute("StartArrow", sp.StartArrow),
            new XAttribute("EndArrow", sp.EndArrow),
            new XAttribute("ArrowSize", sp.ArrowSize),
            new XAttribute("FillShape", sp.FillShape)
        );
    }

    private XElement SerializeSubreportProperties(SubreportProperties sp)
    {
        return new XElement("SubreportProperties",
            new XAttribute("ReportFile", sp.ReportFile),
            new XAttribute("ParameterExpression", sp.ParameterExpression),
            new XAttribute("PrintOnParent", sp.PrintOnParent),
            new XAttribute("KeepTogether", sp.KeepTogether)
        );
    }

    // ============ DESERIALIZATION ============

    public ReportTemplate Deserialize(string frxContent)
    {
        var doc = XDocument.Parse(frxContent);
        var root = doc.Element("Report") ?? throw new InvalidOperationException("Invalid FRX file: missing Report root");

        var template = new ReportTemplate
        {
            Name = root.Attribute("Name")?.Value ?? "Untitled",
            Description = root.Attribute("Description")?.Value ?? string.Empty,
            Author = root.Attribute("Author")?.Value ?? string.Empty,
            ScriptLanguage = root.Attribute("ScriptLanguage")?.Value ?? "CSharp",
            ConvertNulls = bool.TryParse(root.Attribute("ConvertNulls")?.Value, out var cn) && cn,
            DoublePass = bool.TryParse(root.Attribute("DoublePass")?.Value, out var dp) && dp
        };

        if (DateTime.TryParse(root.Attribute("CreatedDate")?.Value, out var cd))
            template.CreatedDate = cd;
        if (DateTime.TryParse(root.Attribute("ModifiedDate")?.Value, out var md))
            template.ModifiedDate = md;

        // Parse Dictionary
        var dictionary = root.Element("Dictionary");
        if (dictionary != null)
        {
            template.DataSources = dictionary.Elements("TableDataSource")
                .Select(ds => DeserializeDataSource(ds)).ToList();
            template.Parameters = dictionary.Elements("Parameter")
                .Select(p => DeserializeParameter(p)).ToList();
        }

        // Parse Styles
        var styles = root.Element("Styles");
        if (styles != null)
        {
            template.Styles = styles.Elements("Style")
                .ToDictionary(
                    s => s.Attribute("Name")?.Value ?? string.Empty,
                    s => s.Attribute("Value")?.Value ?? string.Empty
                );
        }

        // Parse ReportPage
        var reportPage = root.Element("ReportPage");
        if (reportPage != null)
        {
            template.Page = DeserializePageSettings(reportPage);
            template.Bands = reportPage.Elements()
                .Where(e => e.Name.LocalName.EndsWith("Band"))
                .Select(b => DeserializeBand(b))
                .ToList();
        }

        // Parse Script
        template.Script = root.Element("Script")?.Value ?? string.Empty;

        // Parse Referenced Assemblies
        var assemblies = root.Element("ReferencedAssemblies");
        if (assemblies != null)
        {
            template.ReferencedAssemblies = assemblies.Elements("Assembly")
                .Select(a => a.Value)
                .ToList();
        }

        return template;
    }

    private DataSource DeserializeDataSource(XElement element)
    {
        var ds = new DataSource
        {
            Name = element.Attribute("Name")?.Value ?? string.Empty,
            Type = Enum.Parse<DataSourceType>(element.Attribute("DataType")?.Value ?? "Json"),
            Enabled = bool.TryParse(element.Attribute("Enabled")?.Value, out var en) ? en : true,
            CommandTimeout = int.TryParse(element.Attribute("CommandTimeout")?.Value, out var ct) ? ct : 30,
            ConnectionString = element.Attribute("ConnectionString")?.Value ?? string.Empty,
            SelectCommand = element.Attribute("SelectCommand")?.Value ?? string.Empty,
            TableName = element.Attribute("TableName")?.Value ?? string.Empty,
            AssemblyName = element.Attribute("AssemblyName")?.Value ?? string.Empty,
            ClassName = element.Attribute("ClassName")?.Value ?? string.Empty,
            MethodName = element.Attribute("MethodName")?.Value ?? string.Empty,
            Columns = element.Elements("Column").Select(c => DeserializeColumn(c)).ToList(),
            Parameters = element.Elements("Parameter").Select(p => DeserializeParameter(p)).ToList()
        };
        return ds;
    }

    private DataColumn DeserializeColumn(XElement element)
    {
        return new DataColumn
        {
            Name = element.Attribute("Name")?.Value ?? string.Empty,
            DataType = element.Attribute("DataType")?.Value ?? "System.String",
            Caption = element.Attribute("Caption")?.Value ?? string.Empty,
            Size = int.TryParse(element.Attribute("Size")?.Value, out var s) ? s : 0,
            Nullable = bool.TryParse(element.Attribute("Nullable")?.Value, out var n) ? n : true,
            PrimaryKey = bool.TryParse(element.Attribute("PrimaryKey")?.Value, out var pk) && pk,
            ReadOnly = bool.TryParse(element.Attribute("ReadOnly")?.Value, out var ro) && ro,
            Expression = element.Attribute("Expression")?.Value ?? string.Empty,
            Format = element.Attribute("Format")?.Value ?? string.Empty
        };
    }

    private ReportParameter DeserializeParameter(XElement element)
    {
        var param = new ReportParameter
        {
            Name = element.Attribute("Name")?.Value ?? string.Empty,
            DataType = element.Attribute("DataType")?.Value ?? "System.String",
            Required = bool.TryParse(element.Attribute("Required")?.Value, out var r) && r,
            AllowNull = bool.TryParse(element.Attribute("AllowNull")?.Value, out var an) ? an : true,
            Hidden = bool.TryParse(element.Attribute("Hidden")?.Value, out var h) && h,
            MultiValue = bool.TryParse(element.Attribute("MultiValue")?.Value, out var mv) && mv,
            DefaultValue = element.Attribute("DefaultValue")?.Value,
            Description = element.Attribute("Description")?.Value ?? string.Empty,
            Expression = element.Attribute("Expression")?.Value ?? string.Empty,
            LookupDataSource = element.Attribute("LookupDataSource")?.Value ?? string.Empty,
            LookupDisplayField = element.Attribute("LookupDisplayField")?.Value ?? string.Empty,
            LookupValueField = element.Attribute("LookupValueField")?.Value ?? string.Empty
        };

        var allowedValues = element.Element("AllowedValues");
        if (allowedValues != null)
        {
            param.AllowedValues = allowedValues.Elements("Value").Select(v => v.Value).ToList();
        }

        return param;
    }

    private PageSettings DeserializePageSettings(XElement element)
    {
        var ps = new PageSettings
        {
            Width = float.Parse(element.Attribute("Width")?.Value ?? "595", CultureInfo.InvariantCulture),
            Height = float.Parse(element.Attribute("Height")?.Value ?? "842", CultureInfo.InvariantCulture),
            LeftMargin = float.Parse(element.Attribute("LeftMargin")?.Value ?? "40", CultureInfo.InvariantCulture),
            RightMargin = float.Parse(element.Attribute("RightMargin")?.Value ?? "40", CultureInfo.InvariantCulture),
            TopMargin = float.Parse(element.Attribute("TopMargin")?.Value ?? "40", CultureInfo.InvariantCulture),
            BottomMargin = float.Parse(element.Attribute("BottomMargin")?.Value ?? "40", CultureInfo.InvariantCulture),
            Orientation = Enum.Parse<PageOrientation>(element.Attribute("Orientation")?.Value ?? "Portrait"),
            PaperName = element.Attribute("PaperName")?.Value ?? "A4",
            ColumnWidth = float.Parse(element.Attribute("ColumnWidth")?.Value ?? "0", CultureInfo.InvariantCulture),
            ColumnCount = int.Parse(element.Attribute("ColumnCount")?.Value ?? "1"),
            ColumnGap = float.Parse(element.Attribute("ColumnGap")?.Value ?? "0", CultureInfo.InvariantCulture),
            Resolution = int.Parse(element.Attribute("Resolution")?.Value ?? "96")
        };

        var watermark = element.Element("Watermark");
        if (watermark != null)
        {
            ps.WatermarkText = watermark.Attribute("Text")?.Value ?? string.Empty;
            ps.WatermarkFont = watermark.Attribute("Font")?.Value ?? "Arial";
            ps.WatermarkFontSize = float.Parse(watermark.Attribute("FontSize")?.Value ?? "48", CultureInfo.InvariantCulture);
            ps.WatermarkColor = watermark.Attribute("Color")?.Value ?? "#C0C0C0";
            ps.WatermarkOpacity = float.Parse(watermark.Attribute("Opacity")?.Value ?? "0.3", CultureInfo.InvariantCulture);
            ps.WatermarkShowBehind = bool.TryParse(watermark.Attribute("ShowBehind")?.Value, out var sb) ? sb : true;
        }

        return ps;
    }

    private Band DeserializeBand(XElement element)
    {
        var bandType = element.Name.LocalName.Replace("Band", "");
        var band = new Band
        {
            Type = Enum.Parse<BandType>(bandType),
            Name = element.Attribute("Name")?.Value ?? string.Empty,
            Height = float.Parse(element.Attribute("Height")?.Value ?? "37.8", CultureInfo.InvariantCulture),
            Top = float.Parse(element.Attribute("Top")?.Value ?? "0", CultureInfo.InvariantCulture),
            CanGrow = bool.TryParse(element.Attribute("CanGrow")?.Value, out var cg) ? cg : true,
            CanShrink = bool.TryParse(element.Attribute("CanShrink")?.Value, out var cs) && cs,
            RepeatOnEveryPage = bool.TryParse(element.Attribute("RepeatOnEveryPage")?.Value, out var rep) && rep,
            PrintOnBottom = bool.TryParse(element.Attribute("PrintOnBottom")?.Value, out var pb) && pb,
            StartNewPage = bool.TryParse(element.Attribute("StartNewPage")?.Value, out var snp) && snp,
            PrintIfDetailEmpty = bool.TryParse(element.Attribute("PrintIfDetailEmpty")?.Value, out var pide) ? pide : true,
            KeepTogether = bool.TryParse(element.Attribute("KeepTogether")?.Value, out var kt) ? kt : true,
            Outline = bool.TryParse(element.Attribute("Outline")?.Value, out var ol) && ol,
            OutlineLevel = int.TryParse(element.Attribute("OutlineLevel")?.Value, out var olvl) ? olvl : 1,
            DataSourceName = element.Attribute("DataSource")?.Value ?? string.Empty,
            Condition = element.Attribute("Condition")?.Value ?? string.Empty,
            FilterExpression = element.Attribute("Filter")?.Value ?? string.Empty,
            SortExpression = element.Attribute("Sort")?.Value ?? string.Empty,
            GroupExpression = element.Attribute("GroupExpression")?.Value ?? string.Empty,
            OutlineText = element.Attribute("OutlineText")?.Value ?? string.Empty,
            BeforePrintScript = element.Element("BeforePrintScript")?.Value ?? string.Empty,
            AfterPrintScript = element.Element("AfterPrintScript")?.Value ?? string.Empty,
            Objects = element.Elements()
                .Where(e => e.Name.LocalName.EndsWith("Object"))
                .Select(o => DeserializeObject(o))
                .ToList()
        };
        return band;
    }

    private ReportObject DeserializeObject(XElement element)
    {
        var objType = element.Name.LocalName.Replace("Object", "");
        var obj = new ReportObject
        {
            Type = Enum.Parse<ObjectType>(objType),
            Name = element.Attribute("Name")?.Value ?? string.Empty,
            Left = float.Parse(element.Attribute("Left")?.Value ?? "0", CultureInfo.InvariantCulture),
            Top = float.Parse(element.Attribute("Top")?.Value ?? "0", CultureInfo.InvariantCulture),
            Width = float.Parse(element.Attribute("Width")?.Value ?? "100", CultureInfo.InvariantCulture),
            Height = float.Parse(element.Attribute("Height")?.Value ?? "20", CultureInfo.InvariantCulture),
            CanGrow = bool.TryParse(element.Attribute("CanGrow")?.Value, out var cg) ? cg : true,
            CanShrink = bool.TryParse(element.Attribute("CanShrink")?.Value, out var cs) && cs,
            Visible = bool.TryParse(element.Attribute("Visible")?.Value, out var v) ? v : true,
            ZIndex = int.TryParse(element.Attribute("ZIndex")?.Value, out var zi) ? zi : 0,
            DataBinding = element.Attribute("DataColumn")?.Value ?? string.Empty,
            Expression = element.Attribute("Expression")?.Value ?? string.Empty,
            Text = element.Attribute("Text")?.Value ?? string.Empty,
            FormatString = element.Attribute("Format")?.Value ?? string.Empty,
            VisibleExpression = element.Attribute("VisibleExpression")?.Value ?? string.Empty,
            Hyperlink = element.Attribute("Hyperlink")?.Value ?? string.Empty,
            Bookmark = element.Attribute("Bookmark")?.Value ?? string.Empty,
            PrintOn = element.Attribute("PrintOn")?.Value ?? "AllPages",
            Tag = element.Attribute("Tag")?.Value ?? string.Empty
        };

        // Parse TextProperties
        var textProps = element.Element("TextProperties");
        if (textProps != null)
            obj.TextProps = DeserializeTextProperties(textProps);

        // Parse Border
        var border = element.Element("Border");
        if (border != null)
            obj.Border = DeserializeBorderProperties(border);

        // Parse Fill
        var fill = element.Element("Fill");
        if (fill != null)
            obj.Fill = DeserializeFillProperties(fill);

        // Parse type-specific properties
        var picProps = element.Element("PictureProperties");
        if (picProps != null) obj.PictureProps = DeserializePictureProperties(picProps);

        var barProps = element.Element("BarcodeProperties");
        if (barProps != null) obj.BarcodeProps = DeserializeBarcodeProperties(barProps);

        var chartProps = element.Element("ChartProperties");
        if (chartProps != null) obj.ChartProps = DeserializeChartProperties(chartProps);

        var tableProps = element.Element("TableProperties");
        if (tableProps != null) obj.TableProps = DeserializeTableProperties(tableProps);

        var shapeProps = element.Element("ShapeProperties");
        if (shapeProps != null) obj.ShapeProps = DeserializeShapeProperties(shapeProps);

        var subProps = element.Element("SubreportProperties");
        if (subProps != null) obj.SubreportProps = DeserializeSubreportProperties(subProps);

        return obj;
    }

    private TextProperties DeserializeTextProperties(XElement element)
    {
        var fontAttr = element.Attribute("Font")?.Value ?? "Arial, 10pt";
        var fontParts = fontAttr.Split(',');
        var fontName = fontParts[0].Trim();
        var fontSize = 10f;
        if (fontParts.Length > 1)
        {
            var sizeStr = fontParts[1].Trim().Replace("pt", "").Replace("px", "");
            float.TryParse(sizeStr, NumberStyles.Float, CultureInfo.InvariantCulture, out fontSize);
        }

        return new TextProperties
        {
            FontName = fontName,
            FontSize = fontSize,
            Bold = bool.TryParse(element.Attribute("Bold")?.Value, out var b) && b,
            Italic = bool.TryParse(element.Attribute("Italic")?.Value, out var i) && i,
            Underline = bool.TryParse(element.Attribute("Underline")?.Value, out var u) && u,
            Strikeout = bool.TryParse(element.Attribute("Strikeout")?.Value, out var so) && so,
            ForeColor = element.Attribute("ForeColor")?.Value ?? "#000000",
            BackColor = element.Attribute("BackColor")?.Value ?? "#FFFFFF",
            Alignment = Enum.Parse<TextAlignment>(element.Attribute("Alignment")?.Value ?? "Left"),
            WordWrap = bool.TryParse(element.Attribute("WordWrap")?.Value, out var ww) ? ww : true,
            AutoShrink = bool.TryParse(element.Attribute("AutoShrink")?.Value, out var ash) && ash,
            AutoExpand = bool.TryParse(element.Attribute("AutoExpand")?.Value, out var ae) ? ae : true,
            Angle = int.TryParse(element.Attribute("Angle")?.Value, out var a) ? a : 0,
            FormatString = element.Attribute("Format")?.Value ?? string.Empty,
            RightToLeft = bool.TryParse(element.Attribute("RightToLeft")?.Value, out var rtl) && rtl,
            CharSpacing = float.Parse(element.Attribute("CharSpacing")?.Value ?? "0", CultureInfo.InvariantCulture),
            LineSpacing = float.Parse(element.Attribute("LineSpacing")?.Value ?? "1", CultureInfo.InvariantCulture),
            TrimWhitespace = bool.TryParse(element.Attribute("TrimWhitespace")?.Value, out var tw) ? tw : true,
            HtmlTags = bool.TryParse(element.Attribute("HtmlTags")?.Value, out var ht) && ht
        };
    }

    private BorderProperties DeserializeBorderProperties(XElement element)
    {
        return new BorderProperties
        {
            Left = bool.TryParse(element.Attribute("Left")?.Value, out var l) && l,
            Right = bool.TryParse(element.Attribute("Right")?.Value, out var r) && r,
            Top = bool.TryParse(element.Attribute("Top")?.Value, out var t) && t,
            Bottom = bool.TryParse(element.Attribute("Bottom")?.Value, out var b) && b,
            Width = float.Parse(element.Attribute("Width")?.Value ?? "1", CultureInfo.InvariantCulture),
            Color = element.Attribute("Color")?.Value ?? "#000000",
            Style = Enum.Parse<BorderStyle>(element.Attribute("Style")?.Value ?? "Solid"),
            Radius = float.Parse(element.Attribute("Radius")?.Value ?? "0", CultureInfo.InvariantCulture)
        };
    }

    private FillProperties DeserializeFillProperties(XElement element)
    {
        var fp = new FillProperties
        {
            Color = element.Attribute("Color")?.Value ?? "#FFFFFF",
            Type = Enum.Parse<FillType>(element.Attribute("Type")?.Value ?? "Solid")
        };

        if (fp.Type == FillType.Gradient)
        {
            fp.GradientStartColor = element.Attribute("GradientStart")?.Value ?? "#FFFFFF";
            fp.GradientEndColor = element.Attribute("GradientEnd")?.Value ?? "#000000";
            fp.GradientAngle = float.Parse(element.Attribute("GradientAngle")?.Value ?? "0", CultureInfo.InvariantCulture);
        }
        if (fp.Type == FillType.Pattern)
        {
            fp.PatternColor = element.Attribute("PatternColor")?.Value ?? "#000000";
            fp.PatternStyle = element.Attribute("PatternStyle")?.Value ?? "Horizontal";
        }
        if (fp.Type == FillType.Image)
        {
            fp.ImageSource = element.Attribute("ImageSource")?.Value ?? string.Empty;
            fp.ImageTile = bool.TryParse(element.Attribute("ImageTile")?.Value, out var it) && it;
            fp.ImageAlignment = element.Attribute("ImageAlignment")?.Value ?? "Stretch";
        }

        return fp;
    }

    private PictureProperties DeserializePictureProperties(XElement element)
    {
        return new PictureProperties
        {
            ImageSource = element.Attribute("ImageSource")?.Value ?? string.Empty,
            ImageFormat = element.Attribute("ImageFormat")?.Value ?? "png",
            KeepAspectRatio = bool.TryParse(element.Attribute("KeepAspectRatio")?.Value, out var kar) ? kar : true,
            Alignment = element.Attribute("Alignment")?.Value ?? "Center",
            Rotation = int.TryParse(element.Attribute("Rotation")?.Value, out var r) ? r : 0
        };
    }

    private BarcodeProperties DeserializeBarcodeProperties(XElement element)
    {
        return new BarcodeProperties
        {
            BarcodeType = Enum.Parse<BarcodeType>(element.Attribute("BarcodeType")?.Value ?? "Code128"),
            Data = element.Attribute("Data")?.Value ?? string.Empty,
            ShowText = bool.TryParse(element.Attribute("ShowText")?.Value, out var st) ? st : true,
            BarWidth = float.Parse(element.Attribute("BarWidth")?.Value ?? "2", CultureInfo.InvariantCulture),
            BarHeight = float.Parse(element.Attribute("BarHeight")?.Value ?? "50", CultureInfo.InvariantCulture),
            TextPosition = element.Attribute("TextPosition")?.Value ?? "Bottom",
            Checksum = bool.TryParse(element.Attribute("Checksum")?.Value, out var cs) ? cs : true,
            ErrorCorrection = element.Attribute("ErrorCorrection")?.Value ?? "M"
        };
    }

    private ChartProperties DeserializeChartProperties(XElement element)
    {
        return new ChartProperties
        {
            ChartType = Enum.Parse<ChartType>(element.Attribute("ChartType")?.Value ?? "Bar"),
            Title = element.Attribute("Title")?.Value ?? string.Empty,
            XAxisField = element.Attribute("XAxisField")?.Value ?? string.Empty,
            YAxisField = element.Attribute("YAxisField")?.Value ?? string.Empty,
            SeriesField = element.Attribute("SeriesField")?.Value ?? string.Empty,
            ValueField = element.Attribute("ValueField")?.Value ?? string.Empty,
            ShowLegend = bool.TryParse(element.Attribute("ShowLegend")?.Value, out var sl) ? sl : true,
            ShowValues = bool.TryParse(element.Attribute("ShowValues")?.Value, out var sv) ? sv : true,
            ColorPalette = element.Attribute("ColorPalette")?.Value ?? "Default",
            Width = int.TryParse(element.Attribute("Width")?.Value, out var w) ? w : 400,
            Height = int.TryParse(element.Attribute("Height")?.Value, out var h) ? h : 300
        };
    }

    private TableProperties DeserializeTableProperties(XElement element)
    {
        var tp = new TableProperties
        {
            RowCount = int.TryParse(element.Attribute("RowCount")?.Value, out var rc) ? rc : 1,
            ColumnCount = int.TryParse(element.Attribute("ColumnCount")?.Value, out var cc) ? cc : 1,
            RepeatHeaderRow = bool.TryParse(element.Attribute("RepeatHeaderRow")?.Value, out var rhr) ? rhr : true,
            DataSourceName = element.Attribute("DataSource")?.Value ?? string.Empty,
            AutoWidth = bool.TryParse(element.Attribute("AutoWidth")?.Value, out var aw) ? aw : true,
            Columns = element.Elements("Column").Select(c => new TableColumn
            {
                Width = float.Parse(c.Attribute("Width")?.Value ?? "100", CultureInfo.InvariantCulture),
                AutoSize = bool.TryParse(c.Attribute("AutoSize")?.Value, out var @as) ? @as : true,
                FieldName = c.Attribute("FieldName")?.Value ?? string.Empty
            }).ToList(),
            Rows = element.Elements("Row").Select(r => new TableRow
            {
                Height = float.Parse(r.Attribute("Height")?.Value ?? "20", CultureInfo.InvariantCulture),
                IsHeader = bool.TryParse(r.Attribute("IsHeader")?.Value, out var ih) && ih,
                IsFooter = bool.TryParse(r.Attribute("IsFooter")?.Value, out var @if) && @if,
                Cells = r.Elements().Where(e => e.Name.LocalName.EndsWith("Object"))
                    .Select(c => DeserializeTableCell(c)).ToList()
            }).ToList()
        };
        return tp;
    }

    private TableCell DeserializeTableCell(XElement element)
    {
        var cell = new TableCell();
        var baseObj = DeserializeObject(element);

        cell.Id = baseObj.Id;
        cell.Type = baseObj.Type;
        cell.Name = baseObj.Name;
        cell.Left = baseObj.Left;
        cell.Top = baseObj.Top;
        cell.Width = baseObj.Width;
        cell.Height = baseObj.Height;
        cell.DataBinding = baseObj.DataBinding;
        cell.Expression = baseObj.Expression;
        cell.Text = baseObj.Text;
        cell.TextProps = baseObj.TextProps;
        cell.Border = baseObj.Border;
        cell.Fill = baseObj.Fill;

        cell.RowIndex = int.TryParse(element.Attribute("RowIndex")?.Value, out var ri) ? ri : 0;
        cell.ColumnIndex = int.TryParse(element.Attribute("ColumnIndex")?.Value, out var ci) ? ci : 0;
        cell.RowSpan = int.TryParse(element.Attribute("RowSpan")?.Value, out var rs) ? rs : 1;
        cell.ColumnSpan = int.TryParse(element.Attribute("ColumnSpan")?.Value, out var csp) ? csp : 1;
        cell.Aggregate = element.Attribute("Aggregate")?.Value ?? string.Empty;

        return cell;
    }

    private ShapeProperties DeserializeShapeProperties(XElement element)
    {
        return new ShapeProperties
        {
            ShapeType = Enum.Parse<ShapeType>(element.Attribute("ShapeType")?.Value ?? "Rectangle"),
            CornerRadius = int.TryParse(element.Attribute("CornerRadius")?.Value, out var cr) ? cr : 0,
            StartArrow = element.Attribute("StartArrow")?.Value ?? "None",
            EndArrow = element.Attribute("EndArrow")?.Value ?? "None",
            ArrowSize = int.TryParse(element.Attribute("ArrowSize")?.Value, out var ars) ? ars : 5,
            FillShape = bool.TryParse(element.Attribute("FillShape")?.Value, out var fs) ? fs : true
        };
    }

    private SubreportProperties DeserializeSubreportProperties(XElement element)
    {
        return new SubreportProperties
        {
            ReportFile = element.Attribute("ReportFile")?.Value ?? string.Empty,
            ParameterExpression = element.Attribute("ParameterExpression")?.Value ?? string.Empty,
            PrintOnParent = bool.TryParse(element.Attribute("PrintOnParent")?.Value, out var pop) ? pop : true,
            KeepTogether = bool.TryParse(element.Attribute("KeepTogether")?.Value, out var kt) ? kt : true
        };
    }
}

using System.Windows;
using System.Windows.Controls;
using ReportDesigner.Core.Models;

namespace ReportDesigner.Desktop.Converters;

public class ReportObjectTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? TableTemplate { get; set; }
    public DataTemplate? PictureTemplate { get; set; }
    public DataTemplate? BarcodeTemplate { get; set; }
    public DataTemplate? ShapeTemplate { get; set; }
    public DataTemplate? ChartTemplate { get; set; }
    public DataTemplate? DefaultTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is ReportObject reportObj)
        {
            return reportObj.Type switch
            {
                ObjectType.Text => TextTemplate,
                ObjectType.Table => TableTemplate,
                ObjectType.Picture => PictureTemplate,
                ObjectType.Barcode => BarcodeTemplate,
                ObjectType.Shape => ShapeTemplate,
                ObjectType.Chart => ChartTemplate,
                _ => DefaultTemplate
            };
        }
        return DefaultTemplate;
    }
}

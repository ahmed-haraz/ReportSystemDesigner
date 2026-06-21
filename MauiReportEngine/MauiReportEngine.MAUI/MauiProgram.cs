using MauiReportEngine.MAUI.Services;
using Microsoft.Extensions.Logging;

namespace MauiReportEngine.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ITemplateStorageService, TemplateStorageService>();
        builder.Services.AddSingleton<IPrintService, PrintService>();
        builder.Services.AddTransient<IReportService, ReportService>();
        builder.Services.AddTransient<IDataProvider>(sp => 
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            return new SqliteDataProvider(dbPath);
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

namespace MauiReportEngine.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnGenerateReportClicked(object sender, EventArgs e)
    {
        StatusLabel.Text = "Generating report...";
        // Implementation here
        await Task.Delay(1000);
        StatusLabel.Text = "Report generated!";
    }

    private async void OnViewTemplatesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ContentPage { Title = "Templates" });
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ContentPage { Title = "Settings" });
    }
}

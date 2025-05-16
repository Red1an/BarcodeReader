using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;


namespace BarcodeReader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Wrap your shell (or MainPage) in a NavigationPage:

            return app;
        }
    }
}

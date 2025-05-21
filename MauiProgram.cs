using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
#if WINDOWS
using BarcodeReader.Platforms.Windows;
#endif
using BarcodeReader.Controls;

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
                }).ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
handlers.AddHandler(typeof(CameraView), typeof(CameraViewHandler));
#endif
                }); ;
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            return app;
        }
    }
}

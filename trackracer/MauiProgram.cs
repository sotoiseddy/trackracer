using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using trackracer.Models.ViewModels;
using trackracer.Services;

namespace trackracer
{
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
            builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
            ConnectionMultiplexer.Connect("redis-14184.c330.asia-south1-1.gce.redns.redis-cloud.com:14184"));
            builder.Services.AddSingleton<LocationSyncService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

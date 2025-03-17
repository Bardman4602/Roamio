using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Roamio.Mobile.Services;
using System.IO;

namespace Roamio.Mobile
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //SQLite setup
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "tripDb.sqlite");
            builder.Services.AddSingleton<LocalDatabaseService>(new LocalDatabaseService(dbPath));

            // AWS API Base url.
            builder.Services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://vh9kmuogbf.execute-api.eu-north-1.amazonaws.com"); // Add back /Dev if next test doesn't work.
            });

            // Google Maps Client
            builder.Services.AddHttpClient("GoogleMapsClient");

            // DI
            builder.Services.AddTransient<IApiService, ApiService>();
            builder.Services.AddTransient<IGoogleMapsService, GoogleMapsService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            Services = app.Services;

            return app;
        }
    }
}

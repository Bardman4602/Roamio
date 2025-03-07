﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Roamio.Mobile.Services;


namespace Roamio.Mobile
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }

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

            // AWS API Base url.
            builder.Services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://vh9kmuogbf.execute-api.eu-north-1.amazonaws.com/Dev");
            });

            // DI
            builder.Services.AddTransient<IApiService, ApiService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            Services = app.Services;

            return app;
        }
    }
}

using Microsoft.Extensions.Logging;
using OpenAppLab.Core.UI.Shared.GraphQL;
using OpenAppLab.Template.UI.Services;
using OpenAppLab.Template.UI.Shared.Services;

namespace OpenAppLab.Template.UI
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
                });

            // Add device-specific services used by the OpenAppLab.Template.UI.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            string BaseAddress =
                DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:7128/graphql" : "https://localhost:7128/graphql";
            builder.Services.AddHttpClient<GraphQLHttpClientService>(client =>
            {
                client.BaseAddress = new Uri(BaseAddress);
            });

            return builder.Build();
        }
    }
}

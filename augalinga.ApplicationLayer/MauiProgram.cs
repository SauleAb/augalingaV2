using augalinga.Backend.Services;
using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;

namespace augalinga.ApplicationLayer
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
                    fonts.AddFont("Meraki.ttf", "Meraki");
                    fonts.AddFont("Traffolight.otf", "Traffolight");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddScoped<DataContext>();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXlfcnRcR2RfUUx2V0s=");

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();

#endif

            return builder.Build();
        }
    }
}

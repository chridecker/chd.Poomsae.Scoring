using chd.Poomsae.Scoring.App.Platforms.iOS.Authentication;
using chd.Poomsae.Scoring.App.Platforms.iOS.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public static class DIExtensions
    {
        public static IServiceCollection AddiOS(this IServiceCollection services)
        {
            services.ConfigureHttpClientDefaults(builder => builder.ConfigurePrimaryHttpMessageHandler(HttpsClientHandlerService.GetPlatformMessageHandler));
            services.AddSingleton<BLEPeripheralManagerDelegate>();
            services.AddSingleton<NotificationReceiver>();
            services.AddSingleton<INotificationManagerService, NotificationManagerService>();

            services.AddSingleton(_ => AppleSignInAuthenticator.Default);
            services.AddSingleton<FirebaseAuthService>();

            return services;
        }
    }
}

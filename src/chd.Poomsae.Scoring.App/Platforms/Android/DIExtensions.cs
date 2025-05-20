using chd.Poomsae.Scoring.App.Platforms.Android.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using Microsoft.Extensions.Configuration;
using Plugin.Firebase.Auth.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public static class DIExtensions
    {
        public static IServiceCollection AddAndroidServices(this IServiceCollection services)
        {
            services.ConfigureHttpClientDefaults(builder => builder.ConfigurePrimaryHttpMessageHandler(HttpsClientHandlerService.GetPlatformMessageHandler));

            services.AddSingleton<IFirebaseAuthGoogle>(_ => CrossFirebaseAuthGoogle.Current);

            services.AddSingleton<INotificationManagerService, NotificationManagerService>();
            services.AddSingleton<BLEGattCallback>();
            services.AddSingleton<BLEAdvertisingCallback>();
            return services;
        }
    }
}

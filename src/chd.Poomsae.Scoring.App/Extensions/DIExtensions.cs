using Android.App.Roles;
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.App.Services.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.Poomsae.Scoring.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureHttpClientDefaults(builder => builder.ConfigurePrimaryHttpMessageHandler(HttpsClientHandlerService.GetPlatformMessageHandler));

            services.AddSingleton<INotificationManagerService, NotificationManagerService>();
            services.AddSingleton<BLEGattCallback>();
            services.AddSingleton<BLEAdvertisingCallback>();

            //services.AddUi<SettingManager, VibrationHelper, BLEServer>(configuration);
            services.AddUi<SettingManager, VibrationHelper, BLEServerManager>(configuration);

            return services;
        }
    }
}

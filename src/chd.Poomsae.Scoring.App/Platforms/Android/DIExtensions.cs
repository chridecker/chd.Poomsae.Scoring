using chd.Poomsae.Scoring.App.Platforms.Android.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public static class DIExtensions
    {
        public static IServiceCollection AddAndroidServices(this IServiceCollection services)
        {
            services.ConfigureHttpClientDefaults(builder => builder.ConfigurePrimaryHttpMessageHandler(HttpsClientHandlerService.GetPlatformMessageHandler));
            services.AddSingleton<INotificationManagerService, NotificationManagerService>();
            services.AddSingleton<BLEGattCallback>();
            services.AddSingleton<BLEAdvertisingCallback>();
            return services;
        }
    }
}

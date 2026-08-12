using chd.Poomsae.Scoring.App.Platforms.iOS.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;

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
            
            return services;
        }
    }
}

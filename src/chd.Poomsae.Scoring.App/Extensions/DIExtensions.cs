#if ANDROID
using chd.Poomsae.Scoring.App.Platforms.Android;
using chd.Poomsae.Scoring.App.Platforms.Android.Authentication;
using chd.Poomsae.Scoring.App.Platforms.Android.BLE;
#elif IOS
using chd.Poomsae.Scoring.App.Platforms.iOS;
using chd.Poomsae.Scoring.App.Platforms.iOS.BLE;
using chd.Poomsae.Scoring.App.Platforms.iOS.Update;
using chd.Poomsae.Scoring.App.Platforms.iOS.Authentication;
#endif
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chd.Poomsae.Scoring.UI.Services;


namespace chd.Poomsae.Scoring.App.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
#if ANDROID
            services.AddAndroidServices();
            services.AddUi<GoogleSignInManager, MauiUpdateService, DeviceHandler, SettingManager, VibrationHelper, BLEServer, BLEClient, PrintService>(configuration);
#elif IOS
            services.AddiOS();
            services.AddUi<AppleSignInManager, InAppUpdateService, DeviceHandler, SettingManager, VibrationHelper, BLEServer, BLEClient,PrintService>(configuration);
#endif

            services.AddSingleton<IDeviceInfo>(_ => DeviceInfo.Current);
            services.AddSingleton<IAppInfo>(_ => AppInfo.Current);


            return services;
        }
    }
}

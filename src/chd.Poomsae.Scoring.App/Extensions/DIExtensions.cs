#if ANDROID
using chd.Poomsae.Scoring.App.Platforms.Android;
using chd.Poomsae.Scoring.App.Platforms.Android.BLE;
#elif IOS
using chd.Poomsae.Scoring.App.Platforms.iOS;    
using chd.Poomsae.Scoring.App.Platforms.iOS.BLE;    
#endif
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.App.Services.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using Plugin.Firebase.Firestore;
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
#if ANDROID
            services.AddAndroidServices();
            services.AddUi<SettingManager, VibrationHelper, BLEServer, BLEClient>(configuration);
#elif IOS
            services.AddiOS();
            services.AddUi<SettingManager, VibrationHelper, BLEServer, BLEClient>(configuration);   
#endif
             services.AddSingleton<IFirebaseAuth>(_ => CrossFirebaseAuth.Current);
            services.AddSingleton<IFirebaseAuthGoogle>(_ => CrossFirebaseAuthGoogle.Current);
            services.AddSingleton<IFirebaseFirestore>(_ => CrossFirebaseFirestore.Current);
            services.AddSingleton<FirestoreManager>();

            return services;
        }
    }
}

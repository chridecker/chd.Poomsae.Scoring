﻿using chd.Poomsae.Scoring.App.Platforms.Android.Authentication;
using chd.Poomsae.Scoring.App.Platforms.Android.BLE;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using Microsoft.Extensions.Configuration;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using Plugin.Firebase.Firestore;
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

            services.AddSingleton<IFirebaseAuth>(_ => CrossFirebaseAuth.Current);
            services.AddSingleton<IFirebaseAuthGoogle>(_ => CrossFirebaseAuthGoogle.Current);
             services.AddSingleton<IFirebaseFirestore>(_ => CrossFirebaseFirestore.Current);

            services.AddSingleton<IDataService,FireStoreHandler>();

            services.AddSingleton<INotificationManagerService, NotificationManagerService>();
            services.AddSingleton<BLEGattCallback>();
            services.AddSingleton<BLEAdvertisingCallback>();
            return services;
        }
    }
}

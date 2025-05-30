﻿using Blazored.Modal;
using chd.Poomsae.Scoring.App.Extensions;
using chd.Poomsae.Scoring.Contracts.Constants;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using chd.Poomsae.Scoring.App.Settings;
using System.Reflection;
using chd.UI.Base.Contracts.Interfaces.Update;

#if ANDROID
using Maui.Android.InAppUpdates;
using Plugin.Firebase.Core.Platforms.Android;
using Plugin.Firebase.Auth.Google;
#elif IOS
#endif


namespace chd.Poomsae.Scoring.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Configuration.AddConfiguration(GetLocalSetting());
            builder.Configuration.AddConfiguration(GetAppSettingsConfig());
            builder.Services.Configure<GoogleFirebaseSettings>(builder.Configuration.GetSection(nameof(GoogleFirebaseSettings)));

            builder.Services.AddMauiBlazorWebView();
            builder.AddServices();
            builder.RegisterFirebaseServices();
#if ANDROID
            builder.UseAndroidInAppUpdates(options =>
            {
                options.ImmediateUpdatePriority = 6;
            });
#endif

            return builder.Build();
        }
        private static void AddServices(this MauiAppBuilder builder)
        {
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazoredModal();
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Services.AddAppServices(builder.Configuration);
        }
        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                {
                    CrossFirebase.Initialize(activity);
                    FirebaseAuthGoogleImplementation.Initialize(builder.Configuration.GetSection(nameof(GoogleFirebaseSettings))[nameof(GoogleFirebaseSettings.ClientKey)]);
                }));
#elif IOS
               events.AddiOS(iOS => iOS.FinishedLaunching((_, _) =>
               {
                    //CrossFirebase.Initialize();
                    //FirebaseAuthGoogleImplementation.Initialize();

                    var updateSvc = IPlatformApplication.Current.Services.GetRequiredService<IUpdateService>();
                    updateSvc.UpdateAsync(0);
                    return false;
               }));
#endif
            });
            return builder;
        }
        private static IConfiguration GetAppSettingsConfig()
        {
            var fileName = "appsettings.txt";
            if (!FileSystem.AppPackageFileExistsAsync(fileName).Result)
            {
                throw new ApplicationException($"Unable to read file [{fileName}]");
            }
            using var stream = FileSystem.OpenAppPackageFileAsync(fileName).Result;
            return new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
        }

        private static IConfiguration GetLocalSetting()
        {
            if (Preferences.ContainsKey(SettingConstants.License))
            {
                //var pref = Preferences.Default.Get<string>(SettingConstants.LicenseKey, string.Empty);
                var dict = new Dictionary<string, string>();
                return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
            }
            return new ConfigurationBuilder().Build();
        }

    }
}

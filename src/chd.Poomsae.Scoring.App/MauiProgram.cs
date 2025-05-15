using Blazored.Modal;
using chd.Poomsae.Scoring.App.Extensions;
using chd.Poomsae.Scoring.Contracts.Constants;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Firebase;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Firebase;
using chd.Poomsae.Scoring.App.Settings;
using System.Reflection;
using chd.Poomsae.Scoring.Contracts.Settings;
#if ANDROID
using Plugin.Firebase.Core.Platforms.Android;
using Plugin.Firebase.Auth.Platforms.Android.Extensions;
using Plugin.Firebase.Auth.Google;
using Maui.Android.InAppUpdates;
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
#endif
            });
            return builder;
        }
        private static IConfiguration GetAppSettingsConfig()
        {
            var fileName = "appsettings.json";
            var appSettingsFileName = "chdScoring.App.appsettings.json";
            var assembly = Assembly.GetExecutingAssembly();
            using var resStream = assembly.GetManifestResourceStream(appSettingsFileName);
            if (resStream == null)
            {
                throw new ApplicationException($"Unable to read file [{appSettingsFileName}]");
            }
            return new ConfigurationBuilder()
                    .AddJsonStream(resStream)
                    .Build();
        }

        private static IConfiguration GetLocalSetting()
        {
            if (Preferences.ContainsKey(SettingConstants.LicenseKey))
            {
                var pref = Preferences.Default.Get<string>(SettingConstants.LicenseKey, string.Empty);
                var dict = new Dictionary<string, string>()
                {
                    {$"{nameof(LicenseSettings)}:{nameof(LicenseSettings.LicenseKey)}",pref }
                };
                return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
            }
            return new ConfigurationBuilder().Build();
        }

    }
}

using Blazored.Modal;
using chd.Poomsae.Scoring.App.Extensions;
using chd.Poomsae.Scoring.Contracts.Constants;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Maui.Android.InAppUpdates;

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


            builder.Services.AddMauiBlazorWebView();
            builder.AddServices();
            builder.UseAndroidInAppUpdates(options =>
            {
                options.ImmediateUpdatePriority = 6;
            });

            return builder.Build();
        }
        private static void AddServices(this MauiAppBuilder builder)
        {
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazoredModal();
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Services.AddAppServices(builder.Configuration);
        }

        private static IConfiguration GetLocalSetting()
        {
            if (Preferences.ContainsKey(SettingConstants.BaseAddress))
            {
                var pref = Preferences.Default.Get<string>(SettingConstants.BaseAddress, "http://localhost:80");
                var dict = new Dictionary<string, string>()
                {
                    {$"ApiKeys:chdPoomsaeScoring",pref }
                };
                return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
            }
            return new ConfigurationBuilder().Build();
        }
    }
}

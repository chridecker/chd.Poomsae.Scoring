using chd.UI.Base.Client.Implementations.Services.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Update
{
    public class InAppUpdateService : BaseUpdateService
    {


        public InAppUpdateService(ILogger<InAppUpdateService> logger) : base(logger)
        {
        }


        public override async Task UpdateAsync(int timeout)
        {
            var storeVersion = await this.GetAppStoreVersion();
            if (storeVersion > this._currentVersion)
            {
                await Task.Delay(TimeSpan.FromSeconds(timeout));
                await Shell.Current.DisplayAlert("Update verfügbar", "Es gibt eine neue Version im App Store.", "OK");

                // Optional: Zum App Store weiterleiten
                var appStoreUrl = "https://apps.apple.com/app/id<DEINE_APP_ID>"; // z. B. id1234567890
                await Launcher.OpenAsync(appStoreUrl);
            }
        }

        private async Task<Version> GetAppStoreVersion()
        {
            var url = $"https://itunes.apple.com/lookup?bundleId={AppInfo.PackageName}";

            using var client = new HttpClient();
            var json = await client.GetStringAsync(url);
            var data = JsonDocument.Parse(json);
            var root = data.RootElement;

            if (root.GetProperty("resultCount").GetInt32() > 0)
            {
                var appStoreVersion = root
                    .GetProperty("results")[0]
                    .GetProperty("version")
                    .GetString();
                if (Version.TryParse(appStoreVersion, out var store))
                {
                    return store;
                }
            }
            return new Version(1, 0, 0, 0);
        }
    }
}

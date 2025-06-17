using chd.Poomsae.Scoring.Contracts.Interfaces;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public abstract class BaseDeviceHandler : IDeviceHandler
    {
        private readonly IDeviceInfo _deviceInfo;

        public string CurrentAppName => AppInfo.Current.Name;

        public string CurrentAppPackageName => AppInfo.Current.PackageName;

        public string UID => this._nativeUID;

        public Version CurrentVersion => _deviceInfo.Version;

        public int PlatformVersionId => this._nativePlatformVersion;

        public string Platform => this._deviceInfo.Platform.ToString();

        public string Manufacturer => this._deviceInfo.Manufacturer;

        public string Model => this._deviceInfo.Model;

        public string Name => this._deviceInfo.Name;

        public bool IsiOS => this._isiOS;

        protected BaseDeviceHandler(IDeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }

        public void CloseApp() => Application.Current.Quit();


        public Task ShowToast(string message, CancellationToken cancellationToken = default) => Toast.Make(message).Show();


        public abstract Task RequestLandscape();
        public abstract Task ResetOrientation();

        protected abstract bool _isiOS { get; }
        protected abstract string _nativeUID { get; }
        protected abstract int _nativePlatformVersion { get; }


    }
}

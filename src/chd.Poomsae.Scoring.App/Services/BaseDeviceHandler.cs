using chd.Poomsae.Scoring.Contracts.Interfaces;
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

        protected BaseDeviceHandler(IDeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }

        public void CloseApp() => Application.Current.Quit();

        protected abstract string _nativeUID { get; }
        protected abstract int _nativePlatformVersion { get; }


    }
}

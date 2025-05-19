using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using static Android.App.Application;
using static Android.Provider.Settings;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public class DeviceHandler : BaseDeviceHandler
    {
        protected override string _nativeUID => Secure.GetString(Context.ContentResolver, Secure.AndroidId);
        protected override int _nativePlatformVersion =>  (int)Build.VERSION.SdkInt;

        protected override bool _nativeIsiOS => false;

        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {

        }
    }
}

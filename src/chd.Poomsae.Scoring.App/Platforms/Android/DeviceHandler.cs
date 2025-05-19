using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.Settings;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public class DeviceHandler : BaseDeviceHandler
    {
        protected override string NativeUID => Secure.GetString(Platform.CurrentActivity.ContentResolver, Secure.AndroidId);

        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {

        }
    }
}

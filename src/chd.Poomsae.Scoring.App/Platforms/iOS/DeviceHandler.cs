using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public class DeviceHandler : BaseDeviceHandler
    {
        protected override string _nativeUID=> UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        protected override int _nativePlatformVersion => int.TryParse(UIDevice.CurrentDevice.SystemVersion, out var id) ? id : 0;
        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {
            
        }
    }
}

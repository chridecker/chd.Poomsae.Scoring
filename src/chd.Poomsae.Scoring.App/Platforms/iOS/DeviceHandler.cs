using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public class DeviceHandler : BaseDeviceHandler
    {
        protected override string NativeUID=> UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();

        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {
            
        }
    }
}

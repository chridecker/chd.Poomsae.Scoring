using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using Foundation;
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
        protected override string _nativeUID => UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        protected override int _nativePlatformVersion => int.TryParse(UIDevice.CurrentDevice.SystemVersion, out var id) ? id : 0;
        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {

        }

        public override void RequestLandscape()
        {
            UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromInt32((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
        }

        public override void ResetOrientation()
        {
            UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromInt32((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
        }
    }
}

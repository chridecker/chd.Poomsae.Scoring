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

        protected override bool _isiOS => true;

        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {

        }

        public override async Task RequestLandscape()
        {
            await this.SetDeviceOrientation(UIInterfaceOrientation.LandscapeRight);
            UIViewController.AttemptRotationToDeviceOrientation();
        }

        public override async Task ResetOrientation()
        {
            await this.SetDeviceOrientation(UIInterfaceOrientation.Portrait);
            UIViewController.AttemptRotationToDeviceOrientation();
        }
        private async Task SetDeviceOrientation(UIInterfaceOrientation iosOrientation)
        {

            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                var scene = (UIApplication.SharedApplication.ConnectedScenes.ToArray()[0] as UIWindowScene);
                if (scene != null)
                {
                    var uiAppplication = UIApplication.SharedApplication;
                    var test = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    if (test != null)
                    {
                        UIInterfaceOrientationMask NewOrientation;
                        if (iosOrientation == UIInterfaceOrientation.Portrait)
                        {
                            NewOrientation = UIInterfaceOrientationMask.Portrait;
                        }
                        else
                        {
                            NewOrientation = UIInterfaceOrientationMask.Landscape;
                        }
                        scene.Title = "PerformOrientation";
                        scene.RequestGeometryUpdate(
                            new UIWindowSceneGeometryPreferencesIOS(NewOrientation), error => { System.Diagnostics.Debug.WriteLine(error.ToString()); });
                        test.SetNeedsUpdateOfSupportedInterfaceOrientations();
                        test.NavigationController?.SetNeedsUpdateOfSupportedInterfaceOrientations();
                        await Task.Delay(500); //Gives the time to apply the view rotation
                        scene.Title = "";
                    }
                }
            }
            else
            {
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)iosOrientation), new NSString("orientation"));
            }
        }
    }
}

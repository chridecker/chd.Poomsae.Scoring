using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.App.Application;
using static Android.Provider.Settings;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public class DeviceHandler : BaseDeviceHandler
    {
        protected override string _nativeUID => Secure.GetString(Context.ContentResolver, Secure.AndroidId);
        protected override int _nativePlatformVersion => (int)Build.VERSION.SdkInt;
        protected override bool _isiOS => false;

        public DeviceHandler(IDeviceInfo deviceInfo) : base(deviceInfo)
        {

        }

        public override void RequestLandscape()
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

            activity.Window?.AddFlags(WindowManagerFlags.Fullscreen);

            WindowCompat.SetDecorFitsSystemWindows(activity.Window, false);
            WindowInsetsControllerCompat windowInsetsController = new WindowInsetsControllerCompat(activity.Window, activity.Window.DecorView);
            // Hide system bars
            windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;

            activity.RequestedOrientation = ScreenOrientation.SensorLandscape;
        }
        public override void ResetOrientation()
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

            activity.Window?.ClearFlags(WindowManagerFlags.Fullscreen);

            WindowCompat.SetDecorFitsSystemWindows(activity.Window, true);
            WindowInsetsControllerCompat windowInsetsController = new WindowInsetsControllerCompat(activity.Window, activity.Window.DecorView);
            // Hide system bars
            windowInsetsController.Show(WindowInsetsCompat.Type.SystemBars());
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorDefault;

            activity.RequestedOrientation = ScreenOrientation.FullSensor;
        }
    }
}

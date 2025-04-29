using AndroidX.Activity;
using chd.Poomsae.Scoring.Platforms.Android;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.Platform;

namespace chd.Poomsae.Scoring.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.blazorWebView.BlazorWebViewInitialized += this.BlazorWebViewInitialized;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await this.CheckPermissions();
            Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            DeviceDisplay.KeepScreenOn = true;
        }

        private async Task CheckPermissions()
        {
            PermissionStatus notifiatioNpermission = await Permissions.RequestAsync<NotificationPermission>();
            PermissionStatus locationPermission = await Permissions.RequestAsync<LocationPermission>();
            PermissionStatus wifiPermission = await Permissions.RequestAsync<WifiPermission>();
            PermissionStatus blueToothPermission = await Permissions.RequestAsync<BluetoothPermission>();
        }
        private void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            try
            {
                if (e.WebView.Context?.GetActivity() is not ComponentActivity activity)
                {
                    throw new InvalidOperationException($"The permission-managing WebChromeClient requires that the current activity be a '{nameof(ComponentActivity)}'.");
                }

                e.WebView.Settings.JavaScriptEnabled = true;
                e.WebView.Settings.AllowFileAccess = true;
                e.WebView.Settings.MediaPlaybackRequiresUserGesture = false;
                var webChromeClient = new PermissionManagingBlazorWebChromeClient(e.WebView.WebChromeClient!, activity);
                e.WebView.SetWebChromeClient(webChromeClient);
            }
            catch (Exception)
            {
                // do something if error appears
            }

        }
    }
}

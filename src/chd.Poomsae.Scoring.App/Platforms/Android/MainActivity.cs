using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Activity;
using AndroidX.Core.View;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.UI.Base.Contracts.Interfaces.Services;
using Plugin.Firebase.Auth.Google;
using System.Text.Json;

namespace chd.Poomsae.Scoring.App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.SensorLandscape)]
    public class MainActivity : MauiAppCompatActivity
    {
        private readonly IAppInfoService _appInfoService;
        private readonly INotificationManagerService _notificationManagerService;


        public MainActivity()
        {
            this._notificationManagerService = IPlatformApplication.Current.Services.GetService<INotificationManagerService>();
            this._appInfoService = IPlatformApplication.Current.Services.GetService<IAppInfoService>();
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.OnBackPressedDispatcher.AddCallback(this, new BackPress(this._appInfoService));

            this.Window?.AddFlags(WindowManagerFlags.Fullscreen);

            WindowCompat.SetDecorFitsSystemWindows(this.Window, false);
            WindowInsetsControllerCompat windowInsetsController = new WindowInsetsControllerCompat(this.Window, this.Window.DecorView);
            // Hide system bars
            windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            FirebaseAuthGoogleImplementation.HandleActivityResultAsync(requestCode, resultCode, data);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            this.CreateNotificationFromIntent(intent);
        }

        private void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                //var reply = this.GetReply(intent);

                var id = intent.GetIntExtra(NotificationManagerService.IdKey, 0);
                var title = intent.GetStringExtra(NotificationManagerService.TitleKey);
                var message = intent.GetStringExtra(NotificationManagerService.MessageKey);
                var cancel = intent.GetBooleanExtra(NotificationManagerService.CancelKey, false);
                object intentData = null;

                if (intent.HasExtra(NotificationManagerService.DataKey))
                {

                    string data = intent.GetStringExtra(NotificationManagerService.DataKey);
                    string type = intent.GetStringExtra(NotificationManagerService.DataTypeKey);

                    var t = Type.GetType(type);
                    intentData = JsonSerializer.Deserialize(data, t);
                }
                this._notificationManagerService.ReceiveNotification(new NotificationEventArgs(id, title, message, intentData, cancel));
            }
        }

        class BackPress : OnBackPressedCallback
        {
            private readonly IAppInfoService _appInfoService;

            public BackPress(IAppInfoService appInfoService) : base(true)
            {
                this._appInfoService = appInfoService;
            }

            public override void HandleOnBackPressed()
            {
                var navigation = Microsoft.Maui.Controls.Application.Current?.MainPage?.Navigation;
                if (navigation is not null && navigation.ModalStack.Count > 0)
                {
                    Task.Run(navigation.PopModalAsync);
                }
                else
                {

                    this._appInfoService.BackButtonPressed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}

using Foundation;
using UIKit;

namespace chd.Poomsae.Scoring.App
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        //public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        //{
        //    FirebaseApp.Configure(); // <-- Wichtig!
        //    return base.FinishedLaunching(app, options);
        //}
    }
}

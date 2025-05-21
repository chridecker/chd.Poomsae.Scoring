using chd.UI.Base.Contracts.Enum;
using chd.UI.Base.Contracts.Interfaces.Services;
namespace chd.Poomsae.Scoring.App
{
    public partial class App : Application
    {
        private readonly IAppInfoService _appInfoService;
        public App(IAppInfoService appInfoService)
        {
            InitializeComponent();
            //MainPage = new MainPage();
            this._appInfoService = appInfoService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainWindow = new Window(new MainPage());
            mainWindow.Deactivated += (sender, args) => this._appInfoService.AppLifeCycleChanged?.Invoke(this, EAppLifeCycle.OnSleep);
            mainWindow.Resumed += (sender, args) => this._appInfoService.AppLifeCycleChanged?.Invoke(this, EAppLifeCycle.OnResume);

            return mainWindow;
        }
    }
}

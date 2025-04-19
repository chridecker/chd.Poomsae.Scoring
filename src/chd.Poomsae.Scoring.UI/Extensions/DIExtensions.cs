using chd.UI.Base.Client.Implementations.Services.Base;
using chd.UI.Base.Client.Implementations.Services;
using chd.UI.Base.Contracts.Interfaces.Services;
using chd.UI.Base.Contracts.Interfaces.Update;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.UI.Base.Client.Extensions;
using chd.Poomsae.Scoring.UI.Services;

namespace chd.Poomsae.Scoring.UI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddUi<TSettingManager, TVibrationHelper, TBroadCastService>(this IServiceCollection services, IConfiguration configuration)
                 where TSettingManager : BaseClientSettingManager<int, int>, ISettingManager
            where TVibrationHelper : class, IVibrationHelper
            where TBroadCastService : class, IBroadCastService
        {
            services.AddAuthorizationCore();
            services.AddUtilities<chdProfileService, int, int, UserIdLogInService, TSettingManager, ISettingManager, UIComponentHandler, IBaseUIComponentHandler, UpdateService>(ServiceLifetime.Singleton);
            services.AddMauiModalHandler();
            services.AddScoped<INavigationHistoryStateContainer, NavigationHistoryStateContainer>();
            services.AddScoped<INavigationHandler, NavigationHandler>();

            services.AddSingleton<IAppInfoService, AppInfoService>();
            services.AddSingleton<IInitDtoService, InitDtoService>();
            services.AddSingleton<IStartRunService, StartRunService>();
            services.AddSingleton<IVibrationHelper, TVibrationHelper>();
            services.AddSingleton<IBroadCastService, TBroadCastService>();
            return services;
        }
    }
}

﻿using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Persistence;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Client.Extensions;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Client.Implementations.Services;
using chd.UI.Base.Client.Implementations.Services.Base;
using chd.UI.Base.Contracts.Interfaces.Services;
using chd.UI.Base.Contracts.Interfaces.Update;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddUi<TProfileService,  TUpdateService, TDeviceHandler, TSettingManager, TVibrationHelper, TBroadCastService, TBroadcastClient>(this IServiceCollection services, IConfiguration configuration)
                 where TProfileService : ProfileService<Guid, int>, ILicenseTokenProfileService
                 where TSettingManager : BaseClientSettingManager<Guid, int>, ISettingManager
            where TVibrationHelper : class, IVibrationHelper
            where TBroadCastService : class, IBroadCastService
            where TBroadcastClient : class, IBroadcastClient
            where TUpdateService : BaseUpdateService, IUpdateService
            where TDeviceHandler : class, IDeviceHandler
        {
            services.AddAuthorizationCore();
            services.AddUtilities<TProfileService, Guid, int, UserIdLogInService, TSettingManager, ISettingManager, UIComponentHandler, IBaseUIComponentHandler, TUpdateService>(ServiceLifetime.Singleton);
            services.AddMauiModalHandler();
            services.AddScoped<INavigationHistoryStateContainer, NavigationHistoryStateContainer>();
            services.AddScoped<INavigationHandler, NavigationHandler>();

            services.Add(new ServiceDescriptor(typeof(ILicenseTokenProfileService), sp => sp.GetRequiredService<TProfileService>(), ServiceLifetime.Singleton));
            services.AddDataAccess(configuration);

            services.AddSingleton<IDeviceHandler, TDeviceHandler>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IAppInfoService, AppInfoService>();
            services.AddSingleton<IResultService, ResultService>();
            services.AddSingleton<IStartRunService, StartRunService>();
            services.AddSingleton<IFighterDataService, FighterDataService>();
            services.AddSingleton<IVibrationHelper, TVibrationHelper>();
            services.AddSingleton<IBroadCastService, TBroadCastService>();
            services.AddSingleton<IBroadcastClient, TBroadcastClient>();
            return services;
        }
    }
}

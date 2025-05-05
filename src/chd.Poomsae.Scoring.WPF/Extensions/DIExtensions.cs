using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Extensions;
using chd.Poomsae.Scoring.WPF.Services;
using chd.Poomsae.Scoring.WPF.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddChdPoomsaeApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SettingDto>(configuration.GetSection(nameof(SettingDto)));

            services.AddUi<SettingManager, VibrationHelper, TcpBroadcastClient, TcpServer>(configuration);

            services.AddSingleton<INotificationManagerService, NotificationManagerService>();
            return services;
        }
    }
}

using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Extensions;
using chd.Poomsae.Scoring.WPF.Services;
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
            services.AddUi<SettingManager>(configuration);

            services.AddSingleton<INotificationManagerService, NotificationManagerService>();
            return services;
        }
    }
}

﻿using chd.Poomsae.Scoring.Persistence.CompiledModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Persistence
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ScoringContext>(options =>
            {
                System.AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue31751", true);
                options.UseModel(ScoringContextModel.Instance);
                options.UseSqlite(configuration.GetConnectionString(nameof(ScoringContext)));
            });
            return services;
        }
    }
}

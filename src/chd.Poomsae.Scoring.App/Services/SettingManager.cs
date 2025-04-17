﻿using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.UI.Base.Client.Implementations.Services.Base;
using chd.UI.Base.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
     public class SettingManager : BaseClientSettingManager<int, int>, ISettingManager
    {
        public SettingManager(ILogger<SettingManager> logger, IProtecedLocalStorageHandler protecedLocalStorageHandler,
            NavigationManager navigationManager) : base(logger, protecedLocalStorageHandler, navigationManager)
        {
        }

        public Task<T?> GetNativSetting<T>(string key) where T : class
        {
            if (Preferences.ContainsKey(key))
            {
                var settingValue = Preferences.Default.Get<T>(key, default(T));
                return Task.FromResult(settingValue);
            }
            return Task.FromResult(default(T));
        }

        public Task SetNativSetting<T>(string key, T value) where T : class
        {
            Preferences.Default.Set<T>(key, value);
            return Task.CompletedTask;
        }
    }
}

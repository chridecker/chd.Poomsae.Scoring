using chd.UI.Base.Contracts.Interfaces.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface ISettingManager : IBaseClientSettingManager
    {
        Task<string> GetToken();
        Task<string> GetName();
        Task<T?> GetNativSetting<T>(string key) where T : class;
        Task SetName(string name);
        Task SetToken(string name);
        Task SetNativSetting<T>(string key, T value) where T : class;
    }
}

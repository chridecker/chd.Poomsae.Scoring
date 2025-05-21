using chd.UI.Base.Client.Implementations.Services.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class MauiUpdateService : BaseUpdateService
    {
        protected readonly IAppInfo _appInfo;

        public MauiUpdateService(ILogger<BaseUpdateService> logger, IAppInfo appInfo) : base(logger)
        {
            this._appInfo = appInfo;
        }

        public override Task<Version> CurrentVersion()
        {
            return Task.FromResult(this._appInfo.Version);
        }

        public override Task UpdateAsync(int timeout)
        {
            return Task.CompletedTask;
        }
    }
}

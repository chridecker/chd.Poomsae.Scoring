using chd.UI.Base.Client.Implementations.Services.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class UpdateService : BaseUpdateService
    {
        public UpdateService(ILogger<BaseUpdateService> logger) : base(logger)
        {
        }

        public override Task UpdateAsync(int timeout)
        {
            return Task.CompletedTask;
        }
    }
}

using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public abstract class BaseDeviceHandler : IDeviceHandler
    {
        private readonly IDeviceInfo _deviceInfo;

        public string UID => this.NativeUID;

        protected abstract string NativeUID { get; }

        protected BaseDeviceHandler(IDeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }

    }
}

using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class DeviceHandler : IDeviceHandler
    {
        public string UID => throw new NotImplementedException();

        public string CurrentAppName => throw new NotImplementedException();

        public string CurrentAppPackageName => throw new NotImplementedException();

        public Version CurrentVersion => throw new NotImplementedException();

        public int PlatformVersionId => throw new NotImplementedException();

        public string Platform => throw new NotImplementedException();

        public string Manufacturer => throw new NotImplementedException();

        public string Model => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public bool IsiOS => throw new NotImplementedException();

        public void CloseApp()
        {
            throw new NotImplementedException();
        }
    }
}

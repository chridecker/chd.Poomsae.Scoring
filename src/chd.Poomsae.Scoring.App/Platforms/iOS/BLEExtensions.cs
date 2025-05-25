using CoreBluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public static class BLEExtensions
    {
        public static Guid ParseDeviceId(this CBCentral device) => Guid.Parse(device.Identifier.AsString());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BLEResultEventArgs : EventArgs
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public byte[] Data { get; set; }
    }
}

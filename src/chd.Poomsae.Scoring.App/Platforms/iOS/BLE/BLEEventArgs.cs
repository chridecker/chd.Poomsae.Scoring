using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public class BLEEventArgs : EventArgs
    {
        public CBPeripheralManager Peripheral { get; set; }
        public CBATTRequest Request { get; set; }
        public CBCentral Central { get; set; }
        public CBService Service { get; set; }
        public CBCharacteristic Characteristic { get; set; }
        public NSError Error { get; set; }
    }
}

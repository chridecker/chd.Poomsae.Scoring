using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BleEventArgs : EventArgs
    {
        public BluetoothDevice Device { get; set; }
        public GattStatus GattStatus { get; set; }
        public BluetoothGattCharacteristic Characteristic { get; set; }
        public BluetoothGattDescriptor Descriptor{ get; set; }
        public byte[] Value { get; set; }
        public int RequestId { get; set; }
        public int Offset { get; set; }
    }
}

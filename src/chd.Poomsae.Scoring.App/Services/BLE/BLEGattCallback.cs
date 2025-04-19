using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BLEGattCallback : BluetoothGattServerCallback
    {
        public event EventHandler<BleEventArgs> NotificationSent;
        public event EventHandler<BleEventArgs> CharacteristicReadRequest;
        public event EventHandler<BleEventArgs> CharacteristicWriteRequest;
        public BLEGattCallback()
        {

        }
        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
           BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);
            this.CharacteristicReadRequest?.Invoke(this, new BleEventArgs() { Device = device, Characteristic = characteristic, RequestId = requestId, Offset = offset });
        }

        //public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
        //    bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        //{
        //    base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);
        //    this.CharacteristicWriteRequest?.Invoke(this, new BleEventArgs() { Device = device, Characteristic = characteristic, Value = value, RequestId = requestId, Offset = offset });
        //}

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            base.OnNotificationSent(device, status);
            this.NotificationSent?.Invoke(this, new BleEventArgs() { Device = device });
        }
    }
}

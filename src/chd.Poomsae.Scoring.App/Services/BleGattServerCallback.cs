using Android.Bluetooth;
using chd.Poomsae.Scoring.App.Services.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class BleGattServerCallback : BluetoothGattServerCallback
    {

        public event EventHandler<BleEventArgs> NotificationSent;
        public event EventHandler<BleEventArgs> CharacteristicReadRequest;
        public event EventHandler<BleEventArgs> CharacteristicWriteRequest;

        public BleGattServerCallback()
        {

        }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);

            Console.WriteLine("Read request from {0}", device.Name);

            if (CharacteristicReadRequest != null)
            {
                CharacteristicReadRequest(this, new BleEventArgs() { Device = device, Characteristic = characteristic, RequestId = requestId, Offset = offset });
            }
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);

            if (CharacteristicWriteRequest != null)
            {
                CharacteristicWriteRequest(this, new BleEventArgs() { Device = device, Characteristic = characteristic, Value = value, RequestId = requestId, Offset = offset });
            }
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            Console.WriteLine("State changed to {0}", newState);

        }

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            base.OnNotificationSent(device, status);

            if (NotificationSent != null)
            {
                NotificationSent(this, new BleEventArgs() { Device = device });
            }
        }

    }
}

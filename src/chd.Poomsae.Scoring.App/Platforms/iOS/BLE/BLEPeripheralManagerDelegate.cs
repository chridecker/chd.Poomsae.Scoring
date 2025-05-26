using CoreBluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public class BLEPeripheralManagerDelegate : CBPeripheralManagerDelegate
    {
        public event EventHandler<BLEEventArgs> ReadRequest;
        public event EventHandler<BLEEventArgs> StateUpdate;
        public event EventHandler<BLEEventArgs> ServiceAdd;
        public event EventHandler<BLEEventArgs> AdvertisingStart;
        public event EventHandler<BLEEventArgs> CharacteristicSubscribe;
        public event EventHandler<BLEEventArgs> CharacteristicUnsubscribe;

        public override async void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            base.ReadRequestReceived(peripheral, request);
            this.ReadRequest?.Invoke(this, new BLEEventArgs()
            {
                Peripheral = peripheral,
                Request = request,
            });
        }
        public override void StateUpdated(CBPeripheralManager peripheral)
        {
            this.StateUpdate?.Invoke(this, new BLEEventArgs()
            {
                Peripheral = peripheral,
            });
        }
        public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError error)
        {
            this.ServiceAdd?.Invoke(this, new BLEEventArgs()
            {
                Peripheral = peripheral,
                Service = service,
                Error = error,
            });
        }
        public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError error)
        {
            this.AdvertisingStart?.Invoke(this, new BLEEventArgs()
            {
                Peripheral = peripheral,
                Error = error,
            });
        }
        public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            this.CharacteristicSubscribe?.Invoke(this, new BLEEventArgs()
            {
                Peripheral = peripheral,
                Central = central,
                Characteristic = characteristic,
            });
        }
        public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            this.CharacteristicUnsubscribe?.Invoke(this, new BLEEventArgs()
            {
                Peripheral = peripheral,
                Central = central,
                Characteristic = characteristic,
            });
        }
    }
}
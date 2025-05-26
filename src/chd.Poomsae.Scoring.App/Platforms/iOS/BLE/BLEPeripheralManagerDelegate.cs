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
        public event EventHandler<CBATTRequest> ReadRequest;
        public event EventHandler<CBPeripheralManager> StateUpdate;
        public event EventHandler<(CBPeripheralManager, CBService, NSError)> ServiceAdd;
        public event EventHandler<(CBPeripheralManager, NSError)> AdvertisingStart;
        public event EventHandler<(CBPeripheralManager, CBCentral, CBCharacteristic)> CharacteristicSubscribe;
        public event EventHandler<(CBPeripheralManager, CBCentral, CBCharacteristic)> CharacteristicUnsubscribe;

        public override async void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            base.ReadRequestReceived(peripheral, request);
            this.ReadRequest?.Invoke(this, request);
        }
        public override void StateUpdated(CBPeripheralManager peripheral)
        {
            this.StateUpdate?.Invoke(this, peripheral);
        }
        public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError error)
        {
            this.ServiceAdd?.Invoke(this, (peripheral, service, error));
        }
        public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError error)
        {
            this.AdvertisingStart?.Invoke(this, (peripheral, error));
        }
        public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            this.CharacteristicSubscribe?.Invoke(this, (peripheral, central, characteristic));
        }
        public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            this.CharacteristicUnsubscribe?.Invoke(this, (peripheral, central, characteristic));
        }
    }
}
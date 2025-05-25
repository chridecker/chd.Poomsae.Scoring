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

        public override async void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            base.ReadRequestReceived(peripheral, request);
            this.ReadRequest?.Invoke(this, request);
        }
        public override void StateUpdated(CBPeripheralManager peripheral)
        {
            this.StateUpdate?.Invoke(this, peripheral);
        }
    }
}

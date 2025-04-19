using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Provider;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BLEServer : IBroadCastService
    {
        private BluetoothManager _bluetoothManager;
        private BluetoothAdapter _bluetoothAdapter;
        private BLEGattCallback _callback;
        private BluetoothGattServer _gattServer;

        private BluetoothGattService _resultService;
        private BluetoothGattCharacteristic _characteristic;

        public BLEServer(BLEGattCallback callback)
        {
            this._callback = callback;
            this._callback.NotificationSent += this._callback_NotificationSent;
        }

        private void _callback_NotificationSent(object? sender, BleEventArgs e)
        {

        }
        public void BroadcastResult(RunDto run)
        {
            if (run is EliminationRunDto elimination)
            {
                this._characteristic.SetValue([1, __dataConvert(elimination.ChongScore.Accuracy), __dataConvert(elimination.ChongScore.SpeedAndPower), __dataConvert(elimination.ChongScore.RhythmAndTempo), __dataConvert(elimination.ChongScore.ExpressionAndEnergy), 2, __dataConvert(elimination.HongScore.Accuracy), __dataConvert(elimination.HongScore.SpeedAndPower), __dataConvert(elimination.HongScore.RhythmAndTempo), __dataConvert(elimination.HongScore.ExpressionAndEnergy)]);
                foreach (var device in this._gattServer.ConnectedDevices)
                {
                    this._gattServer.NotifyCharacteristicChanged(device, this._characteristic, false);
                }
            }

            byte __dataConvert(decimal d) => (byte)(d * 10);
        }

        public void Start()
        {
            if (this._bluetoothManager is not null) { return; }

            var ctx = Platform.AppContext;
            this._bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            this._bluetoothAdapter = this._bluetoothManager.Adapter;

            this._gattServer = this._bluetoothManager.OpenGattServer(ctx, this._callback);

            this.CreateService();

            var builder = new AdvertiseSettings.Builder();
            builder.SetAdvertiseMode(AdvertiseMode.LowLatency);
            builder.SetConnectable(true);
            builder.SetTimeout(0);
            builder.SetTxPowerLevel(AdvertiseTx.PowerHigh);
            AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder();
            dataBuilder.SetIncludeDeviceName(true);
            dataBuilder.AddServiceUuid(ParcelUuid.FromString(BLEConstants.Result_Gatt_Service.ToString()));
            dataBuilder.SetIncludeTxPowerLevel(true);

            this._bluetoothAdapter.BluetoothLeAdvertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), new BleAdvertiseCallback());
        }

        private void CreateService()
        {
            this._resultService = new BluetoothGattService(UUID.FromString(BLEConstants.Result_Gatt_Service.ToString()), GattServiceType.Primary);

            this._characteristic = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Result_Characteristic.ToString()), GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            this._characteristic.AddDescriptor(new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Result_Descriptor.ToString()),
                     GattDescriptorPermission.Read | GattDescriptorPermission.Write));

            this._resultService.AddCharacteristic(this._characteristic);
            this._gattServer.AddService(this._resultService);
        }

        //private async void ReadRequest(object sender, BleEventArgs e)
        //{
        //    e.Characteristic.SetValue(String.Format("Start!"));
        //    this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue());
        //}
    }
}

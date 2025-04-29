using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Provider;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.UI.Base.Client.Implementations.Services.Base;
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
        private readonly BLEAdvertisingCallback _advertisingCallback;
        private BluetoothGattServer _gattServer;

        private BluetoothGattService _resultService;
        private BluetoothGattCharacteristic _characteristic;
        private BluetoothGattDescriptor _desc;
        private BluetoothGattCharacteristic _characteristicName;

        private List<string> readDevices = [];

        public BLEServer(BLEGattCallback callback, BLEAdvertisingCallback advertisingCallback)
        {
            this._callback = callback;
            this._advertisingCallback = advertisingCallback;
            this._callback.NotificationSent += this._callback_NotificationSent;
            this._callback.CharacteristicReadRequest += this.ReadRequest;
            this._callback.DescriptorReadRequest += this._callback_DescriptorReadRequest;
            this._callback.DescriptorWriteRequest += this._callback_DescriptorWriteRequest;
        }


        private void _callback_NotificationSent(object? sender, BleEventArgs e)
        {
            //this._gattServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
        }

        public void ResetScore()
        {
            this._characteristic.SetValue([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            this.BroadCastToAllDevices();
        }
        public void BroadcastResult(RunDto run)
        {
            if (run is EliminationRunDto elimination)
            {
                this._characteristic.SetValue([1, __dataConvert(elimination.ChongScore.Accuracy), __dataConvert(elimination.ChongScore.SpeedAndPower), __dataConvert(elimination.ChongScore.RhythmAndTempo), __dataConvert(elimination.ChongScore.ExpressionAndEnergy), 2, __dataConvert(elimination.HongScore.Accuracy), __dataConvert(elimination.HongScore.SpeedAndPower), __dataConvert(elimination.HongScore.RhythmAndTempo), __dataConvert(elimination.HongScore.ExpressionAndEnergy)]);
                this.BroadCastToAllDevices();
            }
            else if (run is SingleRunDto singleRun)
            {
                this._characteristic.SetValue([1, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower), __dataConvert(singleRun.Score.RhythmAndTempo), __dataConvert(singleRun.Score.ExpressionAndEnergy), 0, 0, 0, 0, 0]);
                this.BroadCastToAllDevices();
            }

            byte __dataConvert(decimal d) => (byte)(d * 10);
        }

        public async Task StartAsync()
        {

            _ = await Permissions.RequestAsync<BluetoothPermission>();

            if (this._bluetoothManager is not null) { return; }

            var ctx = Platform.AppContext;
            this._bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            this._bluetoothAdapter = this._bluetoothManager.Adapter;

            this._gattServer = this._bluetoothManager.OpenGattServer(ctx, this._callback);

            this.CreateService();
            BluetoothLeAdvertiser advertiser = this._bluetoothAdapter.BluetoothLeAdvertiser;
            var builder = new AdvertiseSettings.Builder();
            builder.SetAdvertiseMode(AdvertiseMode.LowLatency);
            builder.SetConnectable(true);
            builder.SetTimeout(0);
            builder.SetTxPowerLevel(AdvertiseTx.PowerHigh);
            //builder.SetDiscoverable(true);

            AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder();
            dataBuilder.SetIncludeDeviceName(true);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {
                var id = ParcelUuid.FromString(BLEConstants.Result_Gatt_Service.ToString());
                dataBuilder.AddServiceUuid(id);
            }
            dataBuilder.SetIncludeTxPowerLevel(true);
            advertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), this._advertisingCallback);
        }

        private void BroadCastToAllDevices()
        {
            foreach (var device in this._bluetoothManager.GetConnectedDevices(ProfileType.Gatt))
            {
                if (this.readDevices.Any(a => a == device.Address))
                {
                    this._gattServer.NotifyCharacteristicChanged(device, this._characteristic, false);
                }
            }
        }

        private void CreateService()
        {
            this._resultService = new BluetoothGattService(UUID.FromString(BLEConstants.Result_Gatt_Service.ToString()), GattServiceType.Primary);

            this._characteristicName = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Name_Characteristic.ToString()), GattProperty.Read, GattPermission.Read);
            this._characteristic = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Result_Characteristic.ToString()), GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            this._desc = new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Notify_Descriptor.ToString()), GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            this._desc.SetValue(BluetoothGattDescriptor.DisableNotificationValue.ToArray());

            this._characteristic.AddDescriptor(this._desc);

            this._characteristic.SetValue([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            this._resultService.AddCharacteristic(this._characteristic);
            this._resultService.AddCharacteristic(this._characteristicName);
            this._gattServer.AddService(this._resultService);
        }


        private void _callback_DescriptorReadRequest(object? sender, BleEventArgs e)
        {
            if (e.Descriptor.Uuid == this._desc.Uuid)
            {
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Descriptor.GetValue());
            }
        }
        private void _callback_DescriptorWriteRequest(object? sender, BleEventArgs e)
        {
            if (e.Descriptor.Uuid == this._desc.Uuid)
            {
                this._desc.SetValue(e.Value);
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Descriptor.GetValue());
            }
        }

        private async void ReadRequest(object sender, BleEventArgs e)
        {
            if (e.Characteristic.InstanceId == this._characteristic.InstanceId)
            {
            }
            else if (e.Characteristic.InstanceId == this._characteristicName.InstanceId)
            {
                e.Characteristic.SetValue(DeviceInfo.Current.Name);
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue());
                this.readDevices.Add(e.Device.Address);
            }
        }

    }
}

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
        private readonly BLEAdvertisingCallback _advertisingCallback;
        private readonly ISettingManager _settingManager;
        private BluetoothGattServer _gattServer;

        private BluetoothGattService _resultService;
        private BluetoothGattCharacteristic _characteristic;
        private BluetoothGattCharacteristic _characteristicName;

        public BLEServer(BLEGattCallback callback, BLEAdvertisingCallback advertisingCallback, ISettingManager settingManager)
        {
            this._callback = callback;
            this._advertisingCallback = advertisingCallback;
            this._settingManager = settingManager;
            this._callback.NotificationSent += this._callback_NotificationSent;
            this._callback.CharacteristicReadRequest += this.ReadRequest;
            this._callback.DescriptorReadRequest += this._callback_DescriptorReadRequest;
        }


        private void _callback_NotificationSent(object? sender, BleEventArgs e)
        {
            this._gattServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
        }
        public void BroadcastResult(RunDto run)
        {
            if (run is EliminationRunDto elimination)
            {
                this._characteristic.SetValue([1, __dataConvert(elimination.ChongScore.Accuracy), __dataConvert(elimination.ChongScore.SpeedAndPower), __dataConvert(elimination.ChongScore.RhythmAndTempo), __dataConvert(elimination.ChongScore.ExpressionAndEnergy), 2, __dataConvert(elimination.HongScore.Accuracy), __dataConvert(elimination.HongScore.SpeedAndPower), __dataConvert(elimination.HongScore.RhythmAndTempo), __dataConvert(elimination.HongScore.ExpressionAndEnergy)]);
                foreach (var device in this._bluetoothManager.GetConnectedDevices(ProfileType.Gatt))
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
            BluetoothLeAdvertiser advertiser = this._bluetoothAdapter.BluetoothLeAdvertiser;
            var builder = new AdvertiseSettings.Builder();
            builder.SetAdvertiseMode(AdvertiseMode.LowLatency);
            builder.SetConnectable(true);
            builder.SetTimeout(0);
            builder.SetTxPowerLevel(AdvertiseTx.PowerHigh);
            AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder();
            dataBuilder.SetIncludeDeviceName(true);
            //dataBuilder.AddServiceUuid(ParcelUuid.FromString(BLEConstants.Result_Gatt_Service.ToString()));
            dataBuilder.SetIncludeTxPowerLevel(true);

            advertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), this._advertisingCallback);
        }

        private void CreateService()
        {
            this._resultService = new BluetoothGattService(UUID.FromString(BLEConstants.Result_Gatt_Service.ToString()), GattServiceType.Primary);

            this._characteristicName = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Name_Characteristic.ToString()), GattProperty.Read, GattPermission.Read);
            this._characteristic = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Result_Characteristic.ToString()), GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            var desc = new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Notify_Descriptor.ToString()), GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            desc.SetValue(BluetoothGattDescriptor.DisableNotificationValue.ToArray());

            this._characteristic.AddDescriptor(desc);

            this._characteristic.SetValue([1, 0, 0, 0, 0, 2, 0, 0, 0, 0]);
            this._resultService.AddCharacteristic(this._characteristic);
            this._resultService.AddCharacteristic(this._characteristicName);
            this._gattServer.AddService(this._resultService);
        }


        private void _callback_DescriptorReadRequest(object? sender, BleEventArgs e)
        {
            if (e.Descriptor.Uuid == UUID.FromString(BLEConstants.Notify_Descriptor.ToString()))
            {
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Descriptor.GetValue());
            }
        }

        private void ReadRequest(object sender, BleEventArgs e)
        {
            if (e.Characteristic.InstanceId == this._characteristic.InstanceId)
            {
            }
            else if (e.Characteristic.InstanceId == this._characteristicName.InstanceId)
            {
                var name = DeviceInfo.Current.Name + "*";

                var nameTask = this._settingManager.GetSettingLocal(SettingConstants.OwnName);
                Task.WaitAny(nameTask, Task.Delay(TimeSpan.FromSeconds(1)));
                if (nameTask.IsCompleted && !string.IsNullOrWhiteSpace(name))
                {
                    name = nameTask.Result;
                }
                e.Characteristic.SetValue(name);
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue());
            }
        }

    }
}

using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Provider;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Enums;
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
        private readonly ISettingManager _settingManager;
        private BluetoothGattServer _gattServer;

        private BluetoothGattService _resultService;
        private BluetoothGattCharacteristic _characteristic;
        private BluetoothGattDescriptor _descNotifyResult;
        private BluetoothGattDescriptor _descNotifyNameChanged;
        private BluetoothGattCharacteristic _characteristicName;

        private List<string> readDevices = [];

        private byte[] _resultNotifyDescValue = BluetoothGattDescriptor.DisableNotificationValue.ToArray();
        private byte[] _nameNotifyDescValue = BluetoothGattDescriptor.DisableNotificationValue.ToArray();
        private byte[] _resultCharacteristicValue = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        public BLEServer(BLEGattCallback callback, BLEAdvertisingCallback advertisingCallback, ISettingManager settingManager)
        {
            this._callback = callback;
            this._advertisingCallback = advertisingCallback;
            this._settingManager = settingManager;
            this._callback.CharacteristicReadRequest += this.ReadRequest;
            this._callback.DescriptorReadRequest += this._callback_DescriptorReadRequest;
            this._callback.DescriptorWriteRequest += this._callback_DescriptorWriteRequest;
        }

        public async Task BroadcastNameChange()
        {
            var name = await this.GetName();
            this._characteristicName.SetValue(name.Item1);
            this.BroadCastToAllDevices(this._characteristicName, name.Item2);
        }


        public void ResetScore()
        {
            this.SetResultValue([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            this._characteristic.SetValue(this._resultCharacteristicValue);
            this.BroadCastToAllDevices(this._characteristic, this._resultCharacteristicValue);
        }
        public void BroadcastResult(RunDto run)
        {
            if (run is EliminationRunDto elimination)
            {
                this.SetResultValue([1, __dataConvert(elimination.ChongScore.Accuracy), __dataConvert(elimination.ChongScore.SpeedAndPower ?? 0m), __dataConvert(elimination.ChongScore.RhythmAndTempo ?? 0m), __dataConvert(elimination.ChongScore.ExpressionAndEnergy ?? 0m), 2, __dataConvert(elimination.HongScore.Accuracy), __dataConvert(elimination.HongScore.SpeedAndPower ?? 0m), __dataConvert(elimination.HongScore.RhythmAndTempo ?? 0m), __dataConvert(elimination.HongScore.ExpressionAndEnergy ?? 0m)]);
                this._characteristic.SetValue(this._resultCharacteristicValue);
                this.BroadCastToAllDevices(this._characteristic, this._resultCharacteristicValue);
            }
            else if (run is SingleRunDto singleRun)
            {
                if (singleRun.Color is EScoringButtonColor.Blue)
                {
                    this.SetResultValue([1, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower ?? 0m), __dataConvert(singleRun.Score.RhythmAndTempo ?? 0m), __dataConvert(singleRun.Score.ExpressionAndEnergy ?? 0m), 0, 0, 0, 0, 0]);
                }
                else if (singleRun.Color is EScoringButtonColor.Red)
                {
                    this.SetResultValue([0, 0, 0, 0, 0, 2, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower ?? 0m), __dataConvert(singleRun.Score.RhythmAndTempo ?? 0m), __dataConvert(singleRun.Score.ExpressionAndEnergy ?? 0m)]);
                }
                this._characteristic.SetValue(this._resultCharacteristicValue);
                this.BroadCastToAllDevices(this._characteristic, this._resultCharacteristicValue);
            }

            byte __dataConvert(decimal d) => (byte)(d * 10);
        }

        public async Task StartAsync(CancellationToken token)
        {
            var perm = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            while (perm is not PermissionStatus.Granted)
            {
                _ = await Permissions.RequestAsync<BluetoothPermission>();

                perm = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                await Task.Delay(250, token);
                if (token.IsCancellationRequested || perm is PermissionStatus.Granted)
                {
                    break;
                }
            }

            if (this._bluetoothManager is not null) { return; }

            var ctx = Platform.AppContext;
            this._bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            this._bluetoothAdapter = this._bluetoothManager.Adapter;

            this._gattServer = this._bluetoothManager.OpenGattServer(ctx, this._callback);

            await this.CreateService();
            BluetoothLeAdvertiser advertiser = this._bluetoothAdapter.BluetoothLeAdvertiser;
            var builder = new AdvertiseSettings.Builder();
            builder.SetAdvertiseMode(AdvertiseMode.LowLatency);
            builder.SetConnectable(true);
            builder.SetTimeout(0);
            builder.SetTxPowerLevel(AdvertiseTx.PowerHigh);

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
        private void SetResultValue(byte[] data)
        {
            this._resultCharacteristicValue = data;
        }

        private void BroadCastToAllDevices(BluetoothGattCharacteristic characteristic, byte[] value)
        {
            foreach (var device in this._bluetoothManager.GetConnectedDevices(ProfileType.Gatt))
            {
                if (this.readDevices.Any(a => a == device.Address))
                {
                    this.NotifyCharacteristicChange(device, characteristic, false, value);
                }
            }
        }

        private void NotifyCharacteristicChange(BluetoothDevice device, BluetoothGattCharacteristic characteristic, bool confirm, byte[] value)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                this._gattServer.NotifyCharacteristicChanged(device, characteristic, confirm, value);
            }
            this._gattServer.NotifyCharacteristicChanged(device, characteristic, confirm);
        }

        private async Task CreateService()
        {
            this._resultService = new BluetoothGattService(UUID.FromString(BLEConstants.Result_Gatt_Service.ToString()), GattServiceType.Primary);

            this._characteristicName = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Name_Characteristic.ToString()), GattProperty.Read | GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            this._descNotifyNameChanged = new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Notify_Descriptor.ToString()), GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            this._descNotifyNameChanged.SetValue(this._nameNotifyDescValue);

            this._characteristicName.AddDescriptor(this._descNotifyResult);

            var name = await this.GetName();
            this._characteristicName.SetValue(name.Item1);


            this._characteristic = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Result_Characteristic.ToString()), GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            this._descNotifyResult = new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Notify_Descriptor.ToString()), GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            this._descNotifyResult.SetValue(this._resultCharacteristicValue);

            this._characteristic.AddDescriptor(this._descNotifyResult);

            this._characteristic.SetValue(this._resultCharacteristicValue);

            this._resultService.AddCharacteristic(this._characteristic);
            this._resultService.AddCharacteristic(this._characteristicName);

            this._gattServer.AddService(this._resultService);
        }


        private void _callback_DescriptorReadRequest(object? sender, BleEventArgs e)
        {
            if (e.Descriptor.Uuid == this._descNotifyResult.Uuid)
            {
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, this._resultNotifyDescValue);
            }
            else if (e.Descriptor.Uuid == this._descNotifyNameChanged.Uuid)
            {
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, this._nameNotifyDescValue);
            }
        }
        private void _callback_DescriptorWriteRequest(object? sender, BleEventArgs e)
        {
            if (e.Descriptor.Uuid == this._descNotifyResult.Uuid)
            {
                this._resultNotifyDescValue = e.Value;
                this._descNotifyResult.SetValue(this._resultNotifyDescValue);
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, this._resultNotifyDescValue);
            }
            else if (e.Descriptor.Uuid == this._descNotifyNameChanged.Uuid)
            {
                this._nameNotifyDescValue = e.Value;
                this._descNotifyNameChanged.SetValue(this._nameNotifyDescValue);
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, this._nameNotifyDescValue);
            }
        }

        private async void ReadRequest(object sender, BleEventArgs e)
        {
            if (e.Characteristic.InstanceId == this._characteristic.InstanceId)
            {
            }
            else if (e.Characteristic.InstanceId == this._characteristicName.InstanceId)
            {
                var name = await this.GetName();
                e.Characteristic.SetValue(name.Item1);
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, name.Item2);
                this.readDevices.Add(e.Device.Address);
            }
        }

        private async Task<(string, byte[])> GetName()
        {
            var name = await this._settingManager.GetName();
            name = string.IsNullOrWhiteSpace(name) ? DeviceInfo.Current.Name : name;
            return (name, Encoding.ASCII.GetBytes(name));
        }

    }
}

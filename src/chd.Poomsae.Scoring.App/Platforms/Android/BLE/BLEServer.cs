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
using Javax.Security.Auth;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chd.Poomsae.Scoring.App.Services.Base;
using Blazored.Modal.Services;
using chd.Poomsae.Scoring.App.Extensions;


namespace chd.Poomsae.Scoring.App.Platforms.Android.BLE
{
    public class BLEServer : BaseBLEServer<BluetoothDevice, BluetoothGattService, BluetoothGattCharacteristic, BluetoothGattDescriptor>, IBroadCastService
    {
        private BluetoothManager _bluetoothManager;
        private BluetoothAdapter _bluetoothAdapter;
        private BLEGattCallback _callback;
        private readonly BLEAdvertisingCallback _advertisingCallback;
        private BluetoothGattServer _gattServer;


        public BLEServer(BLEGattCallback callback, BLEAdvertisingCallback advertisingCallback, ISettingManager settingManager, IModalService modalService)
            : base(settingManager, modalService, BluetoothGattDescriptor.DisableNotificationValue)
        {
            this._callback = callback;
            this._advertisingCallback = advertisingCallback;

            this._callback.CharacteristicReadRequest += this.ReadRequest;
            this._callback.DescriptorReadRequest += this._callback_DescriptorReadRequest;
            this._callback.DescriptorWriteRequest += this._callback_DescriptorWriteRequest;
            this._callback.DeviceConnectionStateChanged += this._callback_DeviceConnectionStateChanged;
        }

        protected override async Task StartNativeAsync(CancellationToken token)
        {
            _ = await Permissions.RequestAsync<BluetoothPermission>();

            if (this._bluetoothLE.State is not BluetoothState.On or BluetoothState.TurningOn)
            {
                await this._bluetoothLE.TrySetStateAsync(true);
            }

            if (this._bluetoothManager is not null) { return; }

            var ctx = Platform.AppContext;
            this._bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            this._bluetoothAdapter = this._bluetoothManager.Adapter;

            this._gattServer = this._bluetoothManager.OpenGattServer(ctx, this._callback);
            this._gattServer.ClearServices();

            await this.CreateService();
            var advertiser = this._bluetoothAdapter.BluetoothLeAdvertiser;

            var builder = new AdvertiseSettings.Builder();
            builder.SetAdvertiseMode(AdvertiseMode.LowLatency);
            builder.SetConnectable(true);
            builder.SetTimeout(0);
            builder.SetTxPowerLevel(AdvertiseTx.PowerHigh);

            var dataBuilder = new AdvertiseData.Builder();
            dataBuilder.SetIncludeDeviceName(true);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                var id = ParcelUuid.FromString(BLEConstants.Result_Gatt_Service.ToGuidId().ToString());
                dataBuilder.AddServiceUuid(id);
            }
            dataBuilder.SetIncludeTxPowerLevel(true);
            advertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), this._advertisingCallback);
        }

        protected override void BroadCastToAllDevices(BluetoothGattCharacteristic characteristic, byte[] value)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
            {
                characteristic.SetValue(value);
            }
            foreach (var device in this._bluetoothManager.GetConnectedDevices(ProfileType.Gatt))
            {
                if (this._connectedDevices.ContainsKey(device.ParseDeviceId()))
                {
                    this.NotifyCharacteristicChange(device, characteristic, false, value);
                }
            }
        }

        protected override async Task CheckPermissions(CancellationToken cancellationToken)
        {
            var perm = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            while (perm is not PermissionStatus.Granted)
            {
                _ = await Permissions.RequestAsync<Permissions.Bluetooth>();
                _ = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                perm = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                await Task.Delay(250, cancellationToken);
                if (cancellationToken.IsCancellationRequested || perm is PermissionStatus.Granted)
                {
                    break;
                }
            }
        }

        private void NotifyCharacteristicChange(BluetoothDevice device, BluetoothGattCharacteristic characteristic, bool confirm, byte[] value)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                this._gattServer.NotifyCharacteristicChanged(device, characteristic, confirm, value);
            }
            else
            {
                this._gattServer.NotifyCharacteristicChanged(device, characteristic, confirm);
            }
        }

        private async Task CreateService()
        {
            this._service = new BluetoothGattService(UUID.FromString(BLEConstants.Result_Gatt_Service.ToGuidId().ToString()), GattServiceType.Primary);

            this._characteristicName = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Name_Characteristic.ToGuidId().ToString()), GattProperty.Read | GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            this._descNotifyNameChanged = new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Notify_Descriptor.ToGuidId().ToString()), GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            this._descNotifyNameChanged.SetValue(this._nameNotifyDescValue);

            this._characteristicName.AddDescriptor(this._descNotifyNameChanged);

            var name = await this.GetName();
            this._characteristicName.SetValue(name.Item1);


            this._characteristic = new BluetoothGattCharacteristic(UUID.FromString(BLEConstants.Result_Characteristic.ToGuidId().ToString()), GattProperty.Notify, GattPermission.Read | GattPermission.Write);
            this._descNotifyResult = new BluetoothGattDescriptor(UUID.FromString(BLEConstants.Notify_Descriptor.ToGuidId().ToString()), GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            this._descNotifyResult.SetValue(this._resultCharacteristicValue);

            this._characteristic.AddDescriptor(this._descNotifyResult);
            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
            {
                this._characteristic.SetValue(this._resultCharacteristicValue);
            }

            this._service.AddCharacteristic(this._characteristic);
            this._service.AddCharacteristic(this._characteristicName);

            this._gattServer.AddService(this._service);
        }

        private void _callback_DeviceConnectionStateChanged(object? sender, BleEventArgs e)
        {
            if (e.NewState == ProfileState.Disconnected
                && this._connectedDevices.TryRemove(e.Device.ParseDeviceId(), out _))
            {
                this.OnDeviceConnectionChanged(e.Device.ParseDeviceId(), e.Device.Name, false);
            }
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
                if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                {
                    this._descNotifyResult.SetValue(this._resultNotifyDescValue);
                }
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, this._resultNotifyDescValue);
            }
            else if (e.Descriptor.Uuid == this._descNotifyNameChanged.Uuid)
            {
                this._nameNotifyDescValue = e.Value;
                if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                {
                    this._descNotifyNameChanged.SetValue(this._nameNotifyDescValue);
                }
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, this._nameNotifyDescValue);
            }
        }

        private async void ReadRequest(object? sender, BleEventArgs e)
        {
            if (e.Characteristic.InstanceId == this._characteristic.InstanceId)
            {
            }
            else if (e.Characteristic.InstanceId == this._characteristicName.InstanceId)
            {
                var name = await this.GetName();
                if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                {
                    e.Characteristic.SetValue(name.Item1);
                }
                this._gattServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, name.Item2);
                if (!this._connectedDevices.ContainsKey(e.Device.ParseDeviceId()))
                {
                    this._connectedDevices.TryAdd(e.Device.ParseDeviceId(), e.Device);
                    this.OnDeviceConnectionChanged(e.Device.ParseDeviceId(), e.Device?.Name ?? "", true);
                }
            }
        }

    }
}

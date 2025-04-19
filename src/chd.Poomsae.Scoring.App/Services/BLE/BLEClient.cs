using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using Plugin.BLE.Abstractions.EventArgs;
using System.Threading;
using chd.Poomsae.Scoring.Contracts.Constants;

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BLEClient
    {
        private IBluetoothLE _bluetoothLE => CrossBluetoothLE.Current;
        private IAdapter _adapter => this._bluetoothLE.Adapter;

        private event EventHandler<BLEResultEventArgs> ResultReceived;

        public BLEClient()
        {
            this._adapter.DeviceDiscovered += this._adapter_DeviceDiscovered;
            this._adapter.DeviceConnected += this._adapter_DeviceConnected;
        }

        public async Task<bool> StartScanAsync(CancellationToken cancellationToken = default)
        {
            if (this._bluetoothLE.State is not BluetoothState.On or BluetoothState.TurningOn)
            {
                await this._bluetoothLE.TrySetStateAsync(true);
            }
            if (!this._adapter.IsScanning)
            {
                await this._adapter.StartScanningForDevicesAsync(cancellationToken: cancellationToken);
            }
            return this._adapter.IsScanning;
        }

        private async void _adapter_DeviceDiscovered(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            if (device.NativeDevice is BluetoothDevice bDevice)
            {
                _ = await this._adapter.ConnectToKnownDeviceAsync(device.Id);
            }
        }
        private async void _adapter_DeviceConnected(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            if (device.NativeDevice is BluetoothDevice navtiveDevive)
            {
                var service = await device.GetServiceAsync(BLEConstants.Result_Gatt_Service);
                if (service is null) { return; }
                var characteristic = await service.GetCharacteristicAsync(BLEConstants.Result_Characteristic);
                if (characteristic is null || !characteristic.CanUpdate) { return; }
                ;
                characteristic.ValueUpdated += (s, e) => this.Characteristic_ValueUpdated(s, device.Id, device.Name, e);
                await characteristic.StartUpdatesAsync();
            }
        }

        private void Characteristic_ValueUpdated(object? sender, Guid id, string name, CharacteristicUpdatedEventArgs e)
        {
            this.ResultReceived?.Invoke(this, new BLEResultEventArgs()
            {
                DeviceId = id,
                DeviceName = name,
                Data = e.Characteristic.Value
            });
        }
    }
}
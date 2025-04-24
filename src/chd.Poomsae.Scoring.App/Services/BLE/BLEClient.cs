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
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Contracts.Dtos;
using Plugin.BLE.Abstractions;
using Java.Util;

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BLEClient : IBroadcastClient
    {
        private IBluetoothLE _bluetoothLE => CrossBluetoothLE.Current;
        private IAdapter _adapter => this._bluetoothLE.Adapter;

        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        public event EventHandler<DeviceFoundEventArgs> DeviceFound;
        public event EventHandler<Guid> DeviceDisconnected;

        public BLEClient()
        {
            this._adapter.DeviceDiscovered += this._adapter_DeviceDiscovered;
            this._adapter.DeviceConnected += this._adapter_DeviceConnected;
            this._adapter.DeviceDisconnected += this._adapter_DeviceDisconnected;
            this._adapter.ScanTimeoutElapsed += this._adapter_ScanTimeoutElapsed;
        }

        private void _adapter_DeviceDisconnected(object? sender, DeviceEventArgs e)
        {
            this.DeviceDisconnected?.Invoke(this, e.Device.Id);
        }

        private async void _adapter_ScanTimeoutElapsed(object? sender, EventArgs e)
        {
            await this.StartScanAsync();
        }

        public async Task<bool> StartScanAsync(CancellationToken cancellationToken = default)
        {
            if (this._bluetoothLE.State is not BluetoothState.On or BluetoothState.TurningOn)
            {
                await this._bluetoothLE.TrySetStateAsync(true);
            }
            if (!this._adapter.IsScanning)
            {
                await this._adapter.StartScanningForDevicesAsync(new ScanFilterOptions()
                {
                    ServiceUuids = [BLEConstants.Result_Gatt_Service]
                });
            }
            return this._adapter.IsScanning;
        }

        private async void _adapter_DeviceDiscovered(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            if (device.NativeDevice is BluetoothDevice bDevice)
            {
                if (string.IsNullOrWhiteSpace(device.Name)) { return; }
                if (this._adapter.ConnectedDevices.Any(a => a.Id == device.Id)) { return; }
                await this._adapter.ConnectToDeviceAsync(device);
            }
        }
        private async void _adapter_DeviceConnected(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            if (device.NativeDevice is BluetoothDevice navtiveDevive)
            {
                var service = await device.GetServiceAsync(BLEConstants.Result_Gatt_Service);
                if (service is null)
                {
                    await this.DisconnectDevice(device);
                    return;
                }
                var characteristic = await service.GetCharacteristicAsync(BLEConstants.Result_Characteristic);

                if (characteristic is null || !characteristic.CanUpdate)
                {
                    await this.DisconnectDevice(device);
                    return;
                }

                characteristic.ValueUpdated += (s, e) => this.Characteristic_ValueUpdated(s, device.Id, device.Name, e);
                await characteristic.StartUpdatesAsync();

                var characteristicName = await service.GetCharacteristicAsync(BLEConstants.Name_Characteristic);
                var name = device.Name;
                if (characteristicName is not null && characteristicName.CanRead)
                {
                    var data = await characteristicName.ReadAsync();
                    name = Encoding.ASCII.GetString(data.data);
                }

                this.DeviceFound?.Invoke(this, new()
                {
                    Id = device.Id,
                    Name = name,
                    Address = navtiveDevive.Address
                });
            }
        }

        private async Task DisconnectDevice(IDevice device) => this._adapter.DisconnectDeviceAsync(device);

        private async void Characteristic_ValueUpdated(object? sender, Guid id, string name, CharacteristicUpdatedEventArgs e)
        {
            var device = e.Characteristic.Service.Device;
            var service = await device.GetServiceAsync(BLEConstants.Result_Gatt_Service);
            var characteristicName = await service.GetCharacteristicAsync(BLEConstants.Name_Characteristic);

            if (characteristicName is not null && characteristicName.CanRead)
            {
                var data = await characteristicName.ReadAsync();
                name = Encoding.ASCII.GetString(data.data);
            }
            this.ResultReceived?.Invoke(this, new ScoreReceivedEventArgs()
            {
                DeviceId = id,
                DeviceName = name,
                Chong = new(e.Characteristic.Value.Skip(1).Take(4).ToArray()),
                Hong = new(e.Characteristic.Value.Skip(6).Take(4).ToArray()),
            });
        }
    }
}
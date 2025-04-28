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
            this._adapter.DeviceConnectionLost += this._adapter_DeviceDisconnected;
        }

        private void _adapter_DeviceDisconnected(object? sender, DeviceEventArgs e)
        {
            this.DeviceDisconnected?.Invoke(this, e.Device.Id);
        }

        public async Task<Dictionary<Guid, string>> CurrentConnectedDevices(CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var device in this._adapter.ConnectedDevices)
            {
                var readName = await this.ReadNameAsync(device, cancellationToken);
                dict[device.Id] = string.IsNullOrWhiteSpace(readName) ? device.Name : readName;
            }
            return dict;
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
                    ServiceUuids = [BLEConstants.Result_Gatt_Service],
                }, d => !string.IsNullOrWhiteSpace(d.Name) && !this._adapter.ConnectedDevices.Any(a => a.Id == d.Id));
            }
            return this._adapter.IsScanning;
        }

        public async Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (this._adapter.ConnectedDevices.Any(a => a.Id == id))
            {
                var device = this._adapter.ConnectedDevices.FirstOrDefault(x => x.Id == id);
                await this._adapter.DisconnectDeviceAsync(device, cancellationToken);
                return true;
            }
            return false;
        }

        private async void _adapter_DeviceDiscovered(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            if (device.NativeDevice is BluetoothDevice bDevice)
            {
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

                var readName = await this.ReadNameAsync(device, CancellationToken.None);
                var name = string.IsNullOrWhiteSpace(readName) ? device.Name : readName;

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

            var readName = await this.ReadNameAsync(device, CancellationToken.None);
            name = string.IsNullOrWhiteSpace(readName) ? name : readName;

            var chongResult = e.Characteristic.Value[0] == 0 ? null : new ScoreDto(e.Characteristic.Value.Skip(1).Take(4).ToArray());
            var hongResult = e.Characteristic.Value[5] == 0 ? null : new ScoreDto(e.Characteristic.Value.Skip(6).Take(4).ToArray());

            this.ResultReceived?.Invoke(this, new ScoreReceivedEventArgs()
            {
                DeviceId = id,
                DeviceName = name,
                Chong = chongResult,
                Hong = hongResult,
            });
        }

        private async Task<string> ReadNameAsync(IDevice device, CancellationToken cancellationToken)
        {
            var service = await device.GetServiceAsync(BLEConstants.Result_Gatt_Service, cancellationToken);
            var characteristicName = await service.GetCharacteristicAsync(BLEConstants.Name_Characteristic, cancellationToken);
            if (characteristicName is not null && characteristicName.CanRead)
            {
                var data = await characteristicName.ReadAsync(cancellationToken);
                return Encoding.ASCII.GetString(data.data);
            }
            return string.Empty;
        }
    }
}
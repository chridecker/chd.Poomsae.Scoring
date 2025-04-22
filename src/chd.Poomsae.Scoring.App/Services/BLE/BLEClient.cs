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

namespace chd.Poomsae.Scoring.App.Services.BLE
{
    public class BLEClient : IBroadcastClient
    {
        private IBluetoothLE _bluetoothLE => CrossBluetoothLE.Current;
        private IAdapter _adapter => this._bluetoothLE.Adapter;

        private Dictionary<Guid, string> _nameDict = [];

        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;


        public BLEClient()
        {
            this._adapter.DeviceDiscovered += this._adapter_DeviceDiscovered;
            this._adapter.DeviceConnected += this._adapter_DeviceConnected;
            this._adapter.ScanTimeoutElapsed += this._adapter_ScanTimeoutElapsed;
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
                await this._adapter.StartScanningForDevicesAsync(cancellationToken: cancellationToken);
            }
            return this._adapter.IsScanning;
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
                if (service is null) { return; }
                var characteristic = await service.GetCharacteristicAsync(BLEConstants.Result_Characteristic);
                var characteristicName = await service.GetCharacteristicAsync(BLEConstants.Name_Characteristic);

                this._nameDict[device.Id] = "J";
                if (characteristicName is not null && characteristicName.CanRead)
                {
                    var data = await characteristicName.ReadAsync();
                    this._nameDict[device.Id] = Encoding.UTF8.GetString(data.data);
                }

                if (characteristic is null || !characteristic.CanUpdate) { return; }

                characteristic.ValueUpdated += (s, e) => this.Characteristic_ValueUpdated(s, device.Id, device.Name, e);
                await characteristic.StartUpdatesAsync();
            }
        }

        private void Characteristic_ValueUpdated(object? sender, Guid id, string name, CharacteristicUpdatedEventArgs e)
        {
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
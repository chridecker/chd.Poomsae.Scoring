﻿using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.EventArgs;
using System.Threading;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Contracts.Dtos;
using Plugin.BLE.Abstractions;

namespace chd.Poomsae.Scoring.App.Services
{
    public class BLEClient : IBroadcastClient
    {
        private IBluetoothLE _bluetoothLE => CrossBluetoothLE.Current;
        private IAdapter _adapter => this._bluetoothLE.Adapter;

        private PeriodicTimer _periodicTimer;

        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        public event EventHandler<DeviceDto> DeviceDiscovered;
        public event EventHandler<DeviceDto> DeviceFound;
        public event EventHandler<DeviceDto> DeviceDisconnected;
        public event EventHandler<DeviceDto> DeviceNameChanged;
        public event EventHandler ScanTimeout;

        public BLEClient()
        {
            this._adapter.ScanTimeoutElapsed += this._adapter_ScanTimeoutElapsed;
            this._adapter.DeviceConnected += this._adapter_DeviceConnected;
            this._adapter.DeviceDisconnected += this._adapter_DeviceDisconnected;
            this._adapter.DeviceConnectionLost += this._adapter_DeviceDisconnected;

            this._periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            this.Timer();
        }

        private void Timer() => Task.Run(async () =>
        {
            while (await this._periodicTimer.WaitForNextTickAsync())
            {
                foreach (var device in this._adapter.ConnectedDevices)
                {
                    try
                    {
                        _ = await this.ReadNameAsync(device, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        await this.DisconnectDevice(device);
                    }
                }
            }
        });

        private void _adapter_ScanTimeoutElapsed(object? sender, EventArgs e)
        {
            this._adapter.DeviceDiscovered -= this._adapter_DeviceDiscoveredAuto;
            this._adapter.DeviceDiscovered -= this._adapter_DeviceDiscovered;
            this.ScanTimeout?.Invoke(this, e);
        }

        private void _adapter_DeviceDisconnected(object? sender, DeviceEventArgs e)
        {
            this.DeviceDisconnected?.Invoke(this, new() { Id = e.Device.Id, Name = e.Device.Name });
        }

        public async Task<List<DeviceDto>> CurrentConnectedDevices(CancellationToken cancellationToken = default)
        {
            var lst = new List<DeviceDto>();
            foreach (var device in this._adapter.ConnectedDevices)
            {
                try
                {
                    var readName = await this.ReadNameAsync(device, cancellationToken);
                    lst.Add(new()
                    {
                        Id = device.Id,
                        Name = string.IsNullOrWhiteSpace(readName) ? device.Name : readName
                    });
                }
                catch (Exception ex)
                {
                }
            }
            return lst;
        }
        public async Task<bool> StartDiscoverAsync(CancellationToken cancellationToken = default)
        {
            if (this._bluetoothLE.State is not BluetoothState.On or BluetoothState.TurningOn)
            {
                await this._bluetoothLE.TrySetStateAsync(true);
            }
            if (!this._adapter.IsScanning)
            {
                this._adapter.DeviceDiscovered += this._adapter_DeviceDiscovered;
                await this._adapter.StartScanningForDevicesAsync([], d => !string.IsNullOrWhiteSpace(d.Name) && !this._adapter.ConnectedDevices.Any(a => a.Id == d.Id));
            }
            return this._adapter.IsScanning;
        }

        public async Task<bool> StartAutoConnectAsync(CancellationToken cancellationToken = default)
        {
            if (this._bluetoothLE.State is not BluetoothState.On or BluetoothState.TurningOn)
            {
                await this._bluetoothLE.TrySetStateAsync(true);
            }
            if (!this._adapter.IsScanning)
            {
                this._adapter.DeviceDiscovered += this._adapter_DeviceDiscoveredAuto;
                await this._adapter.StartScanningForDevicesAsync(new ScanFilterOptions()
                {
                    ServiceUuids = [BLEConstants.Result_Gatt_Service],
                }, d => !string.IsNullOrWhiteSpace(d.Name) && !this._adapter.ConnectedDevices.Any(a => a.Id == d.Id));
            }
            return this._adapter.IsScanning;
        }

        public async Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (this._adapter.ConnectedDevices.Any(a => a.Id == id && a.State == DeviceState.Connected))
            {
                var device = this._adapter.ConnectedDevices.FirstOrDefault(x => x.Id == id);
                await this._adapter.DisconnectDeviceAsync(device, cancellationToken);
                return true;
            }
            return false;
        }
        public async Task<bool> ConnectDeviceAsync(DeviceDto dto, CancellationToken cancellationToken = default)
        {
            var d = await this._adapter.ConnectToKnownDeviceAsync(dto.Id, new ConnectParameters(), cancellationToken: cancellationToken);
            return d is not null;
        }

        private async void _adapter_DeviceDiscoveredAuto(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            await this._adapter.ConnectToDeviceAsync(device);
        }

        private void _adapter_DeviceDiscovered(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
            this.DeviceDiscovered?.Invoke(this, new DeviceDto
            {
                Id = device.Id,
                Name = device.Name
            });
        }
        private async void _adapter_DeviceConnected(object? sender, DeviceEventArgs e)
        {
            var device = e.Device;
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

            var dto = new DeviceDto()
            {
                Id = device.Id,
                Name = device.Name
            };

            var characteristicName = await service.GetCharacteristicAsync(BLEConstants.Name_Characteristic);
            if (characteristicName.CanUpdate)
            {
                characteristicName.ValueUpdated += (s, e) => this.Name_ValueUpdated(s, dto, e);
                await characteristicName.StartUpdatesAsync();
            }

            characteristic.ValueUpdated += (s, e) => this.Characteristic_ValueUpdated(s, dto, e);
            await characteristic.StartUpdatesAsync();

            var readName = await this.ReadNameAsync(device, CancellationToken.None);
            dto.Name = string.IsNullOrWhiteSpace(readName) ? device.Name : readName;

            this.DeviceFound?.Invoke(this, dto);
        }

        private async Task DisconnectDevice(IDevice device)
        {
            try
            {
                await
                    this._adapter.DisconnectDeviceAsync(device);
            }
            catch { }
        }

        private void Name_ValueUpdated(object? sender, DeviceDto dto, CharacteristicUpdatedEventArgs e)
        {
            var name = Encoding.ASCII.GetString(e.Characteristic.Value);
            this.DeviceNameChanged?.Invoke(this, new DeviceDto
            {
                Id = dto.Id,
                Name = name
            });
        }


        private async void Characteristic_ValueUpdated(object? sender, DeviceDto dto, CharacteristicUpdatedEventArgs e)
        {
            var device = e.Characteristic.Service.Device;

            var readName = await this.ReadNameAsync(device, CancellationToken.None);
            dto.Name = string.IsNullOrWhiteSpace(readName) ? dto.Name : readName;

            var chongResult = e.Characteristic.Value[0] == 0 ? null : new ScoreDto(e.Characteristic.Value.Skip(1).Take(4).ToArray());
            var hongResult = e.Characteristic.Value[5] == 0 ? null : new ScoreDto(e.Characteristic.Value.Skip(6).Take(4).ToArray());

            this.ResultReceived?.Invoke(this, new ScoreReceivedEventArgs()
            {
                Device = dto,
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
                var (data, state) = await characteristicName.ReadAsync(cancellationToken);
                if (state != 0)
                {
                    throw new Exception("Error on Read Operation");
                }
                return Encoding.ASCII.GetString(data);
            }
            return string.Empty;
        }
    }
}
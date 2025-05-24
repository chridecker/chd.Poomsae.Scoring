using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public class BLEClient : IBroadcastClient
    {
        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        public event EventHandler<DeviceDto> DeviceFound;
        public event EventHandler<DeviceDto> DeviceDisconnected;
        public event EventHandler<DeviceDto> DeviceDiscovered;
        public event EventHandler ScanTimeout;
        public event EventHandler<DeviceDto> DeviceNameChanged;

        public async Task<bool> ConnectDeviceAsync(DeviceDto dto, CancellationToken cancellationToken = default)
        {
            return false;
        }

        public async Task<List<DeviceDto>> CurrentConnectedDevices(CancellationToken cancellationToken = default)
        {
            return [];
        }

        public async Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return false;
        }

        public async Task<bool> StartAutoConnectAsync(CancellationToken cancellationToken = default)
        {
            return false;
        }

        public async Task<bool> StartDiscoverAsync(CancellationToken cancellationToken = default)
        {
            return false;
        }
    }
}

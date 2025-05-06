using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IBroadcastClient
    {
        event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        event EventHandler<DeviceDto> DeviceFound;
        event EventHandler<DeviceDto> DeviceDisconnected;
        event EventHandler<DeviceDto> DeviceDiscovered;
        event EventHandler ScanTimeout;
        event EventHandler<DeviceDto> DeviceNameChanged;

        Task<List<DeviceDto>> CurrentConnectedDevices(CancellationToken cancellationToken = default);

        Task<bool> StartAutoConnectAsync(CancellationToken cancellationToken = default);
        Task<bool> StartDiscoverAsync(CancellationToken cancellationToken = default);
        Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ConnectDeviceAsync(DeviceDto dto, CancellationToken cancellationToken = default);
    }
}

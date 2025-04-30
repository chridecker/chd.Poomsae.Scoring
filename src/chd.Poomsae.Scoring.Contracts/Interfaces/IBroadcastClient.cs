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

        Task<List<DeviceDto>> CurrentConnectedDevices(CancellationToken cancellationToken = default);

        Task<bool> StartScanAsync(CancellationToken cancellationToken = default);
        Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

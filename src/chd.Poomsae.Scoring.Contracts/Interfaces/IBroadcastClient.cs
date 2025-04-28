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
        event EventHandler<DeviceFoundEventArgs> DeviceFound;
        event EventHandler<Guid> DeviceDisconnected;

        Task<Dictionary<Guid, string>> CurrentConnectedDevices(CancellationToken cancellationToken = default);

        Task<bool> StartScanAsync(CancellationToken cancellationToken = default);
        Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class BLECLientDummy : IBroadcastClient
    {
        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        public event EventHandler<DeviceFoundEventArgs> DeviceFound;
        public event EventHandler<Guid> DeviceDisconnected;

        public Task<bool> StartScanAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}

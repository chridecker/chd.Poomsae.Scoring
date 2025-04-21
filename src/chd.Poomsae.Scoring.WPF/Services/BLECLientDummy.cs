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

        public Task<bool> StartScanAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}

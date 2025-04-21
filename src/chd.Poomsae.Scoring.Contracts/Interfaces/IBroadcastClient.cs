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

        Task<bool> StartScanAsync(CancellationToken cancellationToken = default);
    }
}

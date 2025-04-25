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
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            this.DeviceFound?.Invoke(this, new DeviceFoundEventArgs()
            {
                Id = id1,
                Name = "D1"
            });


            this.DeviceFound?.Invoke(this, new DeviceFoundEventArgs()
            {
                Id = id2,
                Name = "D1"
            });

            this.ResultReceived?.Invoke(this, new ScoreReceivedEventArgs()
            {
                DeviceId = id1,

            });


            this.ResultReceived?.Invoke(this, new ScoreReceivedEventArgs()
            {
                DeviceId = id2,

            });


            return Task.FromResult(true);
        }
    }
}

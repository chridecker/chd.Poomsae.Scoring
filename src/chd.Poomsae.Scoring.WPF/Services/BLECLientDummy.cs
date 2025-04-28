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

        public async Task<Dictionary<Guid, string>> CurrentConnectedDevices(CancellationToken cancellationToken = default)
        {
            return new Dictionary<Guid, string>();
        }

        public async Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return true;
        }

        public async Task<bool> StartScanAsync(CancellationToken cancellationToken = default)
        {
            var ids = Enumerable.Range(0, 5).Select(i => Guid.NewGuid()).ToArray();

            foreach (var id in ids)
            {
                this.DeviceFound?.Invoke(this, new DeviceFoundEventArgs()
                {
                    Id = id,
                    Name = "D"
                });
            }

            await Task.Delay(TimeSpan.FromSeconds(3));

            foreach (var id in ids)
            {
                this.ResultReceived?.Invoke(this, new()
                {
                    DeviceId = id,
                    DeviceName = "D",
                    Chong = new ScoreDto
                    {
                        Accuracy = new Random().Next(0, 40) * 0.1m,
                        ExpressionAndEnergy = new Random().Next(0, 20) * 0.1m,
                        RhythmAndTempo = new Random().Next(0, 20) * 0.1m,
                        SpeedAndPower = new Random().Next(0, 20) * 0.1m,
                    },
                    Hong = new ScoreDto
                    {
                        Accuracy = new Random().Next(0, 40) * 0.1m,
                        ExpressionAndEnergy = new Random().Next(0, 20) * 0.1m,
                        RhythmAndTempo = new Random().Next(0, 20) * 0.1m,
                        SpeedAndPower = new Random().Next(0, 20) * 0.1m,
                    }
                });
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
            return true;
        }
    }
}

using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class BLEClientDummy : IBroadcastClient
    {
        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        public event EventHandler<DeviceDto> DeviceFound;
        public event EventHandler<DeviceDto> DeviceDisconnected;

        public async Task<List<DeviceDto>> CurrentConnectedDevices(CancellationToken cancellationToken = default)
        {
            return [];
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
                this.DeviceFound?.Invoke(this, new()
                {
                    Id = id,
                    Name = $"S21 von Christoph"
                });
            }

            await Task.Delay(TimeSpan.FromSeconds(3));

            foreach (var id in ids)
            {
                this.ResultReceived?.Invoke(this, new()
                {
                    Device = new() { Id = id },
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

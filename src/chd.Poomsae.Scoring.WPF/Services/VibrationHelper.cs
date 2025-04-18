using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class VibrationHelper : IVibrationHelper
    {
        public void Vibrate(TimeSpan duration)
        {
        }

        public Task Vibrate(int repeat, TimeSpan duration, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Vibrate(int repeat, TimeSpan duration, TimeSpan breakDuration, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

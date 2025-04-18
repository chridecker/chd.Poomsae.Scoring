using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IVibrationHelper
    {
        void Vibrate(TimeSpan duration);
        Task Vibrate(int repeat, TimeSpan duration, CancellationToken cancellationToken = default);
        Task Vibrate(int repeat, TimeSpan duration, TimeSpan breakDuration, CancellationToken cancellationToken = default);
    }
}

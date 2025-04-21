using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ScoreReceivedEventArgs : EventArgs
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public ScoreDto Chong { get; set; }
        public ScoreDto Hong { get; set; }
    }
}

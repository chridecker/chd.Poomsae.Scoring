using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ScoreReceivedEventArgs : EventArgs
    {
        public DeviceDto Device{ get; set; }
        public ScoreDto Chong { get; set; }
        public ScoreDto Hong { get; set; }
    }
}

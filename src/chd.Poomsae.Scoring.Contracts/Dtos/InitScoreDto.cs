using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class InitScoreDto
    {
        public decimal TotalMax { get; set; }
        public decimal StartAccuracy { get; set; } = 4;
        public decimal TotalMaxPresentation => this.TotalMax - this.StartAccuracy;
        public decimal SpeedAndPowerMax { get; set; } = 2;
        public decimal RhythmMax { get; set; } = 2;
        public decimal ExpressionOfEnerfyMax { get; set; } = 2;
    }
}

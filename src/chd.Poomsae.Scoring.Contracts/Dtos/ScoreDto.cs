using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ScoreDto
    {
        public decimal Total => this.Accuracy + this.Presentation;
        public decimal Accuracy { get; set; } = 4;
        public decimal Presentation => this.SpeedAndPower + this.RhythmAndTempo + this.ExpressionAndEnergy;
        public decimal SpeedAndPower { get; set; } = 2;
        public decimal RhythmAndTempo { get; set; } = 2;
        public decimal ExpressionAndEnergy { get; set; } = 2;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class PresentatioScoreDto
    {
        public decimal SpeedAndPower { get; set; }
        public decimal RhythmAndTempo { get; set; }
        public decimal ExpressionAndEnergy { get; set; }
    }
}

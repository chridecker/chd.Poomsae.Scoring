using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ScoreDto
    {
        public decimal MaxTotal { get; set; }
        public decimal Total => this.Accuracy + this.Presentation;
        public decimal Accuracy { get; set; }
        public decimal Presentation => this.SpeedAndPower + this.RhythmAndTempo + this.ExpressionAndEnergy;
        public decimal SpeedAndPower { get; set; }
        public decimal RhythmAndTempo { get; set; }
        public decimal ExpressionAndEnergy { get; set; }
        public ScoreDto()
        {

        }
        public ScoreDto(InitScoreDto initDto)
        {
            this.MaxTotal = initDto.TotalMax;
            this.Accuracy = initDto.StartAccuracy;
        }
    }
}

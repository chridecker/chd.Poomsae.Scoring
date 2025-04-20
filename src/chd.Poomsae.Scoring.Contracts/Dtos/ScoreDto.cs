using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ScoreDto
    {
        public decimal Total => this.Accuracy + this.Presentation;
        public decimal Accuracy { get; set; }
        public decimal Presentation => this.SpeedAndPower + this.RhythmAndTempo + this.ExpressionAndEnergy;
        public decimal SpeedAndPower { get; set; }
        public decimal RhythmAndTempo { get; set; }
        public decimal ExpressionAndEnergy { get; set; }
        public ScoreDto()
        {

        }
        public ScoreDto(byte[] data)
        {
            if (data.Length < 4) { return; }
            this.Accuracy = data[0] * 0.1m;
            this.SpeedAndPower = data[1] * 0.1m;
            this.RhythmAndTempo = data[2] * 0.1m;
            this.ExpressionAndEnergy = data[3] * 0.1m;
        }
        public ScoreDto(InitScoreDto initDto)
        {
            this.Accuracy = initDto.StartAccuracy;
        }
    }
}

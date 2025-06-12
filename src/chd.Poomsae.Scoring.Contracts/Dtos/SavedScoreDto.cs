using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class SavedScoreDto
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(Round))]
        public Guid RoundId { get; set; }
        public RoundDto Round { get; set; }
        public Guid JudgeId { get; set; }
        public string? JudgeName { get; set; }
        public decimal Total => this.Accuracy + (this.Presentation ?? 0m);
        public decimal Accuracy { get; set; }
        public decimal? Presentation => this.SpeedAndPower + this.RhythmAndTempo + this.ExpressionAndEnergy;
        public decimal? SpeedAndPower { get; set; }
        public decimal? RhythmAndTempo { get; set; }
        public decimal? ExpressionAndEnergy { get; set; }
        public SavedScoreDto()
        {
            Id = Guid.NewGuid();
        }
    }
}

using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class EliminationRunDto : RunDto
    {
        public FighterDto ChongFighter { get; set; } = new();
        public FighterDto HongFighter { get; set; } = new();

        public ScoreDto ChongScore { get; set; } = new();
        public ScoreDto HongScore { get; set; } = new();
    }
}

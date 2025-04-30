using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class RunResultDto
    {
        public ScoreDto? ChongScore { get; set; }
        public ScoreDto? HongScore { get; set; }
    }
}

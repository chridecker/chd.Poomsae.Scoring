using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class SingleRunDto : RunDto
    {
        public ScoreDto Score { get; set; }
    }
}

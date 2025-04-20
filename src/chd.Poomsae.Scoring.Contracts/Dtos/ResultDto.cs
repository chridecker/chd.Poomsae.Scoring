using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ResultDto
    {
        public Dictionary<Guid, RunResultDto> Results { get; set; } = [];
        public ERunState State { get; set; }
    }
}

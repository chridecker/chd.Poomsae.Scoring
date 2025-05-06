using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos.Base
{
    public abstract class RunDto
    {
        public ERunState State { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos.Base
{
    public abstract class RunDto
    {
        public JudgeDto Judge { get; set; } = new();
    }
}

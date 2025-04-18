using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos.Base
{
    public abstract class RunDto
    {
        public JudgeDto Judge { get; set; } = new();
        public TimeSpan Time { get; set; } = TimeSpan.Zero;
        public ERunState State { get; set; }
    }
}

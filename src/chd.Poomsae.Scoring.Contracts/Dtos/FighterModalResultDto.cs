using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class FighterModalResultDto
    {
        public FighterDto Fighter { get; set; }
        public EDataAction Action { get; set; }
        public FighterModalResultDto(FighterDto fighter, EDataAction action)
        {
            this.Fighter = fighter;
            this.Action = action;
        }
    }
}

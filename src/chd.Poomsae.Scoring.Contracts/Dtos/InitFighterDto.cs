using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class InitFighterDto
    {
        public string ChongFirstname { get; set; } = "Player";
        public string HongFirstname { get; set; } = "Player";
        public string ChongLastname { get; set; } = "1";
        public string HongLastname { get; set; } = "2";
    }
}

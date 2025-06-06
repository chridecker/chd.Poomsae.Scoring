﻿using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IFighterDataService
    {
        List<FighterDto> Fighters { get; }
        Task AddFighter(FighterDto fighter);
        Task RemoveFighter(FighterDto fighter);
        Task UpdateFighter(FighterDto fighter);


        FighterDto CurrentBlue { get; set; }
        FighterDto CurrentRed { get; set; }
    }
}

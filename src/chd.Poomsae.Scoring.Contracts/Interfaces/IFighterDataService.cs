using chd.Poomsae.Scoring.Contracts.Dtos;
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
        void AddFighter(FighterDto fighter);
        void RemoveFighter(FighterDto fighter);
    }
}

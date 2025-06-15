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
        Task AddFighter(FighterDto fighter);
        Task RemoveFighter(FighterDto fighter);
        Task RemoveRound(RoundDto round);
        Task RemoveScore(SavedScoreDto score);
        Task UpdateFighter(FighterDto fighter);
        Task UpdateRound(RoundDto round);
        Task HandleResult(ScoreDto score, DeviceDto device, FighterDto fighterDto);
        Task CreateRound(string name, FighterDto fighter);
        Task CloseRound(FighterDto fighter);

        Task SetBlue(FighterDto fighterDto);
        Task SetRed(FighterDto fighterDto);

        FighterDto CurrentBlue { get; }
        FighterDto CurrentRed { get; }
    }
}

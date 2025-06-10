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
        Task HandleResult(ScoreReceivedEventArgs e);
        Task CreateRound(FighterDto fighter);
        Task CloseRound(FighterDto fighter);

        FighterDto CurrentBlue { get; set; }
        FighterDto CurrentRed { get; set; }
    }
}

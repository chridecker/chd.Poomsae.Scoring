using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class FighterDataService : IFighterDataService
    {
        private readonly ScoringContext _scoringContext;

        public List<FighterDto> Fighters => this._scoringContext.Fighters.ToList();

        public FighterDto CurrentBlue { get; set; }
        public FighterDto CurrentRed { get; set; }

        public FighterDataService(ScoringContext scoringContext)
        {
            this._scoringContext = scoringContext;
        }

        public async Task AddFighter(FighterDto fighter)
        {
            await this._scoringContext.Fighters.AddAsync(fighter);
            await this._scoringContext.SaveChangesAsync();
        }
        public async Task RemoveFighter(FighterDto fighter)
        {
            this._scoringContext.Fighters.Remove(fighter);
            await this._scoringContext.SaveChangesAsync();
        }
        public async Task UpdateFighter(FighterDto fighter)
        {
            this._scoringContext.Fighters.Update(fighter);
            await this._scoringContext.SaveChangesAsync();
        }
    }
}

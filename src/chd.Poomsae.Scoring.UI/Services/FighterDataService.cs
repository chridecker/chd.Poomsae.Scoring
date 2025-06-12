using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Persistence;
using chd.UI.Base.Extensions;
using Microsoft.EntityFrameworkCore;
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

        public List<FighterDto> Fighters => this._scoringContext.Fighters.LoadFighters().ToList();

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
        public Task RemoveFighter(FighterDto fighter) => this.RemoveEntry(fighter);

        public Task RemoveRound(RoundDto round) => this.RemoveEntry(round);

        public Task RemoveScore(SavedScoreDto score) => this.RemoveEntry(score);

        public async Task UpdateFighter(FighterDto fighter)
        {
            this._scoringContext.Fighters.Update(fighter);
            await this._scoringContext.SaveChangesAsync();
        }
        public async Task UpdateRound(RoundDto round)
        {
            this._scoringContext.Rounds.Update(round);
            await this._scoringContext.SaveChangesAsync();
        }

        public async Task CreateRound(FighterDto fighter)
        {
            await this.CloseRound(fighter);
            if (!fighter.Rounds.Any(a => !a.Finished.HasValue))
            {
                await this._scoringContext.Rounds.AddAsync(new RoundDto
                {
                    Created = DateTime.Now,
                    FighterId = fighter.Id,
                });
                await this._scoringContext.SaveChangesAsync();
            }
        }

        public async Task CloseRound(FighterDto fighter)
        {
            if (fighter.Rounds.Any(a => !a.Finished.HasValue && a.Scores.Any()))
            {
                var round = fighter.Rounds.FirstOrDefault(x => !x.Finished.HasValue);
                round.Finished = DateTime.Now;
                this._scoringContext.Update(round);
                await this._scoringContext.SaveChangesAsync();
            }
        }

        public async Task HandleResult(ScoreReceivedEventArgs e)
        {
            if (e.Hong is not null && this.CurrentRed is not null)
            {
                var round = this.CurrentRed.Rounds.Where(x => !x.Finished.HasValue).OrderByDescending(o => o.Created).FirstOrDefault();
                if (round is null) { return; }

                await this._scoringContext.Scores.AddAsync(new SavedScoreDto
                {
                    RoundId = round.Id,
                    JudgeId = e.Device.Id,
                    JudgeName = e.Device.Name,
                    Accuracy = e.Hong.Accuracy,
                    ExpressionAndEnergy = e.Hong.ExpressionAndEnergy,
                    RhythmAndTempo = e.Hong.RhythmAndTempo,
                    SpeedAndPower = e.Hong.SpeedAndPower,
                });

                await this._scoringContext.SaveChangesAsync();
            }
            if (e.Chong is not null && this.CurrentBlue is not null)
            {
                var round = this.CurrentBlue.Rounds.Where(x => !x.Finished.HasValue).OrderByDescending(o => o.Created).FirstOrDefault();
                if (round is null) { return; }

                await this._scoringContext.Scores.AddAsync(new SavedScoreDto
                {
                    RoundId = round.Id,
                    JudgeId = e.Device.Id,
                    JudgeName = e.Device.Name,
                    Accuracy = e.Chong.Accuracy,
                    ExpressionAndEnergy = e.Chong.ExpressionAndEnergy,
                    RhythmAndTempo = e.Chong.RhythmAndTempo,
                    SpeedAndPower = e.Chong.SpeedAndPower,
                });

                await this._scoringContext.SaveChangesAsync();
            }

        }
        private async Task RemoveEntry(object entry)
        {
            this._scoringContext.Remove(entry);
            await this._scoringContext.SaveChangesAsync();
        }

    }
}

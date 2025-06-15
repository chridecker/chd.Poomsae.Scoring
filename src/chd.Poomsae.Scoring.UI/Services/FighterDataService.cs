using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Enums;
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
        private readonly IBroadcastClient _broadcastClient;

        public List<FighterDto> Fighters => this._scoringContext.Fighters.LoadFighters().ToList();

        private FighterDto _currentBlue;
        private FighterDto _currentRed;
        public FighterDto CurrentBlue => this._currentBlue;

        public FighterDto CurrentRed => this._currentRed;

        public FighterDataService(ScoringContext scoringContext, IBroadcastClient broadcastClient)
        {
            this._scoringContext = scoringContext;
            this._broadcastClient = broadcastClient;
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

        public async Task CreateRound(string name, FighterDto fighter)
        {
            await this.CloseRound(fighter);
            if (!fighter.Rounds.Any(a => !a.Finished.HasValue))
            {
                await this._scoringContext.Rounds.AddAsync(new RoundDto
                {
                    Name = name,
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
        public async Task SetBlue(FighterDto fighterDto)
        {
            if (this._currentRed?.Id == fighterDto?.Id)
            {
                this._currentRed = null;
            }
            this._currentBlue = fighterDto;
            await this.Send();
        }

        public async Task SetRed(FighterDto fighterDto)
        {
            if (this.CurrentBlue?.Id == fighterDto?.Id)
            {
                this._currentBlue = null;
            }
            this._currentRed = fighterDto;
            await this.Send();
        }

        private async Task Send()
        {
            foreach (var device in await this._broadcastClient.CurrentConnectedDevices())
            {
                await this._broadcastClient.SendFighter(this.CurrentBlue, EScoringButtonColor.Blue, device);
                await this._broadcastClient.SendFighter(this.CurrentRed, EScoringButtonColor.Red, device);
            }
        }


        public async Task HandleResult(ScoreDto score, DeviceDto device, FighterDto fighterDto)
        {
            if (score is not null && fighterDto is not null)
            {
                var round = fighterDto.Rounds.Where(x => !x.Finished.HasValue).OrderByDescending(o => o.Created).FirstOrDefault();
                if (round is null) { return; }

                await this._scoringContext.Scores.AddAsync(new SavedScoreDto
                {
                    RoundId = round.Id,
                    JudgeId = device.Id,
                    JudgeName = device.Name,
                    Accuracy = score.Accuracy,
                    ExpressionAndEnergy = score.ExpressionAndEnergy,
                    RhythmAndTempo = score.RhythmAndTempo,
                    SpeedAndPower = score.SpeedAndPower,
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

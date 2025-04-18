using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class StartRunService : IStartRunService
    {
        private readonly IInitDtoService _initScoreService;

        public StartRunService(IInitDtoService initScoreService)
        {
            this._initScoreService = initScoreService;
        }
        public EliminationRunDto StartEliminiationRun()
        {
            var dto = new EliminationRunDto()
            {
                ChongFighter = new(this._initScoreService.FighterDto.ChongFirstname,this._initScoreService.FighterDto.ChongLastname),
                HongFighter = new(this._initScoreService.FighterDto.HongFirstname,this._initScoreService.FighterDto.HongLastname),
               
                ChongScore = new(this._initScoreService.ScoreDto),
                HongScore = new(this._initScoreService.ScoreDto),
                State = ERunState.Initial
            };

            return dto;
        }
    }
}

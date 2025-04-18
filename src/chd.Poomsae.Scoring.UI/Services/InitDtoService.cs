using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class InitDtoService : IInitDtoService
    {
        private readonly InitScoreDto _scoreDto;
        private readonly InitFighterDto _fighterDto;


        public InitScoreDto ScoreDto => this._scoreDto;

        public InitFighterDto FighterDto => this._fighterDto;

        public InitDtoService()
        {
            this._scoreDto = new();
            this._fighterDto = new();
        }

    }
}

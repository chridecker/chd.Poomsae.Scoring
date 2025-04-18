using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IInitDtoService
    {
        InitScoreDto ScoreDto { get; }
        InitFighterDto FighterDto { get; }
    }
}

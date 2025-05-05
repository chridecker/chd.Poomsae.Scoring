using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IStartRunService
    {
        EliminationRunDto StartEliminiationRun();
        SingleRunDto StartSingleRun(EScoringButtonColor color);
    }
}

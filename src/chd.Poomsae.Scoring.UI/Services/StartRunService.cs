using chd.Poomsae.Scoring.Contracts.Constants;
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

        public StartRunService()
        {
        }
        public EliminationRunDto StartEliminiationRun()
        {
            var dto = new EliminationRunDto()
            {
                ChongScore = new()
                {
                    Accuracy = ScoreConstants.MaxAccuracy
                },
                HongScore = new()
                {
                    Accuracy = ScoreConstants.MaxAccuracy
                },
                State = ERunState.Started
            };

            return dto;
        }

        public SingleRunDto StartSingleRun(EScoringButtonColor color)
        {
            var dto = new SingleRunDto()
            {
                Score = new()
                {
                    Accuracy = ScoreConstants.MaxAccuracy
                },
                State = ERunState.Started,
                Color = color
            };

            return dto;
        }
    }
}

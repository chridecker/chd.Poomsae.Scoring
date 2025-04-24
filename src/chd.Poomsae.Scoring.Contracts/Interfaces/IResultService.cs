using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IResultService
    {
        ResultDto Result { get; }
        decimal HongPresentation { get; }
        decimal HongAccuracy { get; }
        decimal ChongPresentation { get; }
        decimal ChongAccuracy { get; }
        decimal ChongTotal { get; }
        decimal HongTotal { get; }

        void SetRun(Guid judgeId, RunResultDto dto);
        event EventHandler<ResultDto> ResultChanged;
        event EventHandler<ERunState> StateChanged;
    }

}

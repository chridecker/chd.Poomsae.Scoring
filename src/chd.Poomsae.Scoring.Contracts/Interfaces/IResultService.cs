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
        void SetRun(Guid judgeId, RunResultDto dto);
        event EventHandler<ResultDto> ResultChanged;
        event EventHandler<ERunState> StateChanged;
    }

}

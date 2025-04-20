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
    public class ResultService : IResultService
    {
        private ResultDto _resultDto = new();

        public ResultDto Result => this._resultDto;

        public event EventHandler<ResultDto> ResultChanged;
        public event EventHandler<ERunState> StateChanged;


        public void SetRun(Guid id, RunResultDto runResultDto)
        {
            this._resultDto.Results[id] = runResultDto;
            this.ResultChanged?.Invoke(this, this._resultDto);
        }

    }
}

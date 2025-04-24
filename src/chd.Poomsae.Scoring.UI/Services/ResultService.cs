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

        public decimal ChongTotal => this.ChongAccuracy + this.ChongPresentation;
        public decimal HongTotal => this.HongAccuracy + this.HongPresentation;

        public decimal ChongAccuracy
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return 0m; }
                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Accuracy).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Accuracy);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }
        public decimal ChongPresentation
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return 0m; }
                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Presentation).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Presentation);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }
        public decimal HongAccuracy
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return 0m; }
                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.HongScore).Select(s => s.Accuracy).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.HongScore).Select(s => s.Accuracy);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }
        public decimal HongPresentation
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return 0m; }
                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.HongScore).Select(s => s.Presentation).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.HongScore).Select(s => s.Presentation);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }

    }
}

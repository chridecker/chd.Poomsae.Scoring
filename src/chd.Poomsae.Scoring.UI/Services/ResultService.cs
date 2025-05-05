using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using DocumentFormat.OpenXml.Presentation;
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
        public event EventHandler ResultReceived;

        public void SetRun(Guid id, RunResultDto runResultDto)
        {
            if (this._resultDto.Results.ContainsKey(id)
                && (runResultDto.ChongScore is not null || runResultDto.HongScore is not null))
            {
                if (runResultDto.ChongScore is not null)
                {
                    this._resultDto.Results[id].ChongScore = runResultDto.ChongScore;
                }
                if (runResultDto.HongScore is not null)
                {
                    this._resultDto.Results[id].HongScore = runResultDto.HongScore;
                }
            }
            else
            {
                this._resultDto.Results[id] = runResultDto;
            }
            this.ResultReceived?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            this._resultDto.Results.Clear();
            this.ResultReceived?.Invoke(this, EventArgs.Empty);
        }

        public decimal? ChongTotal => this.ChongAccuracy + this.ChongPresentation;
        public decimal? HongTotal => this.HongAccuracy + this.HongPresentation;

        public decimal? ChongAccuracy
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return null; }
                if (Result.Results.Values.Any(a => a.ChongScore is null)) { return null; }
                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Accuracy).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Accuracy);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }
        public decimal? ChongPresentation
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return null; }
                if (Result.Results.Values.Any(a => a.ChongScore is null)) { return null; }

                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Presentation).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.ChongScore).Select(s => s.Presentation);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }
        public decimal? HongAccuracy
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return null; }
                if (Result.Results.Values.Any(a => a.HongScore is null)) { return null; }

                if (Result.Results.Count <= 3)
                {
                    return this.Result.Results.Values.Select(s => s.HongScore).Select(s => s.Accuracy).Average();
                }
                var accLst = this.Result.Results.Values.Select(s => s.HongScore).Select(s => s.Accuracy);
                return (accLst.Sum() - accLst.Min() - accLst.Max()) / (accLst.Count() - 2);
            }
        }
        public decimal? HongPresentation
        {
            get
            {
                if (!Result.Results?.Values.Any() ?? false) { return null; }
                if (Result.Results.Values.Any(a => a.HongScore is null)) { return null; }

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

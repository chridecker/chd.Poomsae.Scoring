using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Enum;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using chd.Poomsae.Scoring.Contracts.Constants;

namespace chd.Poomsae.Scoring.UI.Components.Pages.Base
{
    public abstract class BaseScoringComponent<TRunDto> : ComponentBase, IDisposable
        where TRunDto : RunDto
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] protected IStartRunService _runService { get; set; }
        [Inject] protected IModalService _modal { get; set; }
        [Inject] IBroadCastService broadCastService { get; set; }

        protected TRunDto runDto;

        private IDisposable _registerLocationChangeHandler;

        protected override async Task OnInitializedAsync()
        {
            this._registerLocationChangeHandler = this._navigationManager.RegisterLocationChangingHandler(OnLocationChanging);
            this.runDto = this.CreateDto();
            await base.OnInitializedAsync();
        }

        protected abstract TRunDto CreateDto();
        protected abstract ScoreDto HandleScore(EScoringButtonColor color);
        protected abstract Task<bool> HandleStartedState();

        protected async Task<TimeSpan?> CalcScore(EScoringButtonColor color, decimal value)
        {
            if (this.runDto.State is not ERunState.Started) { return null; }
            this.CalculateAccuracyScore(this.HandleScore(color), value);
            await this.InvokeAsync(this.StateHasChanged);
            return Math.Abs(value) == 0.1m ? TimeSpan.FromMilliseconds(200) : TimeSpan.FromMilliseconds(300);
        }

        protected async Task HandleClick()
        {
            if (this.runDto.State is ERunState.Started)
            {
                if (!await this.HandleStartedState()) { return; }

                if (this.broadCastService.ConnectedDevices > 0)
                {
                    await this.SendResults();
                }
                this.runDto.State = ERunState.Finished;
            }
            else if (this.runDto.State is ERunState.Finished)
            {
                this.ResetResults();
            }
            await this.InvokeAsync(this.StateHasChanged);
        }



        protected void CheckPresentationScoreForNull(ScoreDto dto)
        {
            if (!dto.SpeedAndPower.HasValue)
            {
                dto.SpeedAndPower = 0;
            }
            if (!dto.ExpressionAndEnergy.HasValue)
            {
                dto.ExpressionAndEnergy = 0;
            }
            if (!dto.RhythmAndTempo.HasValue)
            {
                dto.RhythmAndTempo = 0;
            }
        }
        private void ResetResults()
        {
            this.broadCastService.ResetScore();
            this.runDto = this.CreateDto();
        }

        private async Task SendResults()
        {
            this.broadCastService.BroadcastResult(this.runDto);
            _ = await this._modal.ShowDialog(TextConstants.ScoresSend, EDialogButtons.OK);
        }
        private void CalculateAccuracyScore(ScoreDto dto, decimal value)
        {
            if (dto is null) { return; }
            if (dto.Accuracy - value <= 0)
            {
                dto.Accuracy = 0;
            }
            else if (dto.Accuracy - value >= ScoreConstants.MaxAccuracy)
            {
                dto.Accuracy = ScoreConstants.MaxAccuracy;
            }
            else
            {
                dto.Accuracy -= value;
            }
        }

        private async ValueTask OnLocationChanging(LocationChangingContext context)
        {
            if (this.runDto.State is ERunState.Started)
            {
                var res = await this._modal.ShowDialog(TextConstants.LeaveSiteQuestion, EDialogButtons.YesNo);
                if (res != EDialogResult.Yes)
                {
                    context.PreventNavigation();
                }
            }
            else
            {
                this.broadCastService.ResetScore();
            }
        }

        public void Dispose()
        {
            if (this._registerLocationChangeHandler is not null)
            {
                this._registerLocationChangeHandler.Dispose();
            }
        }
    }
}

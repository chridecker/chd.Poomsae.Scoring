﻿using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Components.Layout;
using chd.Poomsae.Scoring.UI.Components.Shared;
using chd.Poomsae.Scoring.UI.Extensions;
using chd.UI.Base.Client.Implementations.Services;
using chd.UI.Base.Components.Base;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Enum;
using chd.UI.Base.Contracts.Interfaces.Authentication;
using chd.UI.Base.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Components.Pages.Base
{
    public abstract class BaseScoringComponent<TRunDto> : BaseNavigationComponent
        where TRunDto : RunDto
    {
        [Inject] protected IStartRunService _runService { get; set; }
        [Inject] IBroadCastService broadCastService { get; set; }

        protected TRunDto runDto;
        protected string blueName = string.Empty;
        protected string redName = string.Empty;

        protected bool _isLicensed = false;

        protected override bool _showBackButton => true;
        protected override string _navigationBackTitle => "";
        protected override string _navigationTitle => "";

        protected override async Task OnInitializedAsync()
        {
            await this._deviceHandler.RequestLandscape();

            this.blueName = this.broadCastService.BlueName;
            this.redName = this.broadCastService.RedName;

            this.broadCastService.BlueNameReceived += this.BroadCastService_BlueNameReceived;
            this.broadCastService.RedNameReceived += this.BroadCastService_RedNameReceived;

            this.runDto = this.CreateDto();
            await base.OnInitializedAsync();
        }

        private async void BroadCastService_BlueNameReceived(object? sender, string e)
        {
            this.blueName = e;
            await this.InvokeAsync(this.StateHasChanged);
        }

        private async void BroadCastService_RedNameReceived(object? sender, string e)
        {
            this.redName = e;
            await this.InvokeAsync(this.StateHasChanged);
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
            if (!this._profileService.HasUserRight([RightConstants.IS_ALLOWED])) { return; }

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
                await this.ResetResults();
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
        private async Task ResetResults()
        {
            var res = await this._modalService.ShowYesNoDialog(TextConstants.ResetScoreQuestion, this._deviceHandler.IsiOS);
            if (res is EDialogResult.Yes)
            {
                this.broadCastService.ResetScore();
                this.runDto = this.CreateDto();
            }
        }

        private async Task SendResults()
        {
            this.broadCastService.BroadcastResult(this.runDto);
            await this._deviceHandler.ShowToast(TextConstants.ScoresSend, this._cts.Token);
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

        protected override async ValueTask<bool> OnLocationChanging()
        {
            if (this.runDto.State is ERunState.Started)
            {
                var res = await this._modalService.ShowYesNoDialog(TextConstants.LeaveSiteQuestion, this._deviceHandler.IsiOS);
                if (res is not EDialogResult.Yes)
                {
                    return false;
                }
            }
            return true;
        }

        protected override async ValueTask ChangeLocation()
        {
            if (this.runDto.State is not ERunState.Started)
            {
                this.broadCastService.ResetScore();
            }
            await base.ChangeLocation();
        }

        public override void Dispose()
        {
            this.broadCastService.BlueNameReceived -= this.BroadCastService_BlueNameReceived;
            this.broadCastService.RedNameReceived -= this.BroadCastService_RedNameReceived;

            base.Dispose();
        }
    }
}

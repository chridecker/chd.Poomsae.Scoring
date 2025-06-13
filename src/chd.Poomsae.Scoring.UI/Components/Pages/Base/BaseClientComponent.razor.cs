using Blazored.Modal;
using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Components.Shared;
using chd.Poomsae.Scoring.UI.Components.Shared.Result;
using chd.Poomsae.Scoring.UI.Extensions;
using chd.UI.Base.Client.Implementations.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Enum;
using chd.UI.Base.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Components.Pages.Base
{
    public abstract class BaseClientComponent : BaseNavigationComponent
    {

        [Inject] protected IResultService resultService { get; set; }
        [Inject] protected IBroadcastClient broadcastClient { get; set; }

        protected ConcurrentDictionary<Guid, DeviceDto> _connectedDevices = [];

        protected IModalReference _loadingModal;

        protected override bool _showBackButton => true;
        protected override string _navigationBackTitle => "Home";


        protected override async Task OnInitializedAsync()
        {
            this.broadcastClient.ResultReceived += ResultReceived;
            this.broadcastClient.DeviceFound += DeviceFound;
            this.broadcastClient.DeviceDisconnected += DeviceDisconnect;
            this.broadcastClient.ScanTimeout += this.ScanFinished;
            this.broadcastClient.DeviceNameChanged += this.DeviceFound;
            foreach (var d in await this.broadcastClient.CurrentConnectedDevices(this._cts.Token))
            {
                this._connectedDevices[d.Id] = d;
            }
            await base.OnInitializedAsync();
        }

        protected abstract Task OnResultReceived(ScoreReceivedEventArgs dto);
        protected abstract Task OnDeviceDisconncted(DeviceDto dto);

        protected async Task Discover()
        {
            var result = await this._modalService.Show<DiscoverDevices>("Geräte Suchen", new ModalOptions()
            {
                Size = ModalSize.ExtraLarge
            }).Result;
            if (result.Confirmed && result.Data is List<DeviceDto> lst)
            {
                foreach (var d in lst)
                {
                    await this.broadcastClient.ConnectDeviceAsync(d);
                }
            }
        }

        protected async Task RemoveDevices()
        {
            var result = await this._modalService.Show<SelectConnectedDevices>("Geräte Entfernen", new ModalOptions()
            {
                Size = ModalSize.ExtraLarge
            }).Result;
            if (result.Confirmed && result.Data is List<DeviceDto> lst)
            {
                foreach (var d in lst)
                {
                    await this.RemoveDevice(d);
                }
            }
        }

        protected async Task RemoveDevice(DeviceDto device)
        {
            await this.broadcastClient.DisconnectDeviceAsync(device.Id, this._cts.Token);
        }

        protected async Task Search()
        {
            this._loadingModal = this._modalService.ShowLoading("");
            await this.Clear();
            await Task.WhenAny(this.broadcastClient.StartAutoConnectAsync(this._cts.Token), Task.Delay(TimeSpan.FromSeconds(2), this._cts.Token));
        }

        protected virtual async Task Clear()
        {
            this.resultService.Clear();

            this._connectedDevices.Clear();
            foreach (var device in await this.broadcastClient.CurrentConnectedDevices(this._cts.Token))
            {
                await this.RemoveDevice(device);

            }
            await this.InvokeAsync(this.StateHasChanged);
        }
        protected virtual async Task OnDeviceFound(DeviceDto e)
        {
            if (this._connectedDevices.ContainsKey(e.Id))
            {
                this._connectedDevices[e.Id].Name = e.Name;
            }
            else
            {
                this._connectedDevices.TryAdd(e.Id, e);
            }
            await this.InvokeAsync(this.StateHasChanged);
        }

        private async void DeviceDisconnect(object? sender, DeviceDto e)
        {
            if (this._connectedDevices.TryRemove(e.Id, out _))
            {
                await this.OnDeviceDisconncted(e);
            }
        }

        private async void ResultReceived(object? sender, ScoreReceivedEventArgs e)
        {
            this.resultService.SetRun(e.Device.Id, new()
            {
                ChongScore = e.Chong,
                HongScore = e.Hong
            });

            await this.OnResultReceived(e);
        }

        private async void ScanFinished(object? sender, EventArgs e)
        {
            if (this._loadingModal != null)
            {
                this._loadingModal.Close(ModalResult.Ok());
                this._loadingModal = null;
            }
        }
        private async void DeviceFound(object? sender, DeviceDto e)
        {
            await this.OnDeviceFound(e);
        }


        protected override async ValueTask<bool> OnLocationChanging()
        {
            if (this._connectedDevices.Any())
            {
                var res = await this._modalService.ShowYesNoDialog(TextConstants.LeaveSiteQuestion);
                if (res == EDialogResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        public override void Dispose()
        {

            this.broadcastClient.ResultReceived -= ResultReceived;
            this.broadcastClient.DeviceFound -= DeviceFound;
            this.broadcastClient.DeviceNameChanged -= DeviceFound;
            this.broadcastClient.DeviceDisconnected -= DeviceDisconnect;
            this.broadcastClient.ScanTimeout -= this.ScanFinished;

            base.Dispose();
        }
    }
}

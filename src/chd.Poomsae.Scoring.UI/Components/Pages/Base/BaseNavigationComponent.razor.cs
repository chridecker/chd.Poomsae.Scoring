using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Components.Shared;
using chd.UI.Base.Client.Implementations.Services;
using chd.UI.Base.Components.Base;
using chd.UI.Base.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Components.Pages.Base
{
    public abstract class BaseNavigationComponent : PageComponentBase<Guid, int>, IDisposable
    {
        [CascadingParameter] protected CascadingBackButton _backButton { get; set; }

        [Inject] INavigationHandler _navigationManager { get; set; }
        [Inject] protected IModalHandler _modalService { get; set; }
        [Inject] protected IDeviceHandler _deviceHandler { get; set; }


        private IDisposable _registerLocationChangeHandler;
        protected CancellationTokenSource _cts = new();

        protected abstract bool _showBackButton { get; }
        protected abstract string _navigationBackTitle { get; }
        protected abstract string _navigationTitle { get; }

        protected override async Task OnInitializedAsync()
        {
            await this._backButton.SetBackButton(this._showBackButton);
            this._backButton.BackTitle = this._navigationBackTitle;
            this._backButton.Title = this._navigationTitle;


            this._registerLocationChangeHandler = this._navigationManager.RegisterLocationChangingHandler(OnLocationChanging, ChangeLocation);
            await base.OnInitializedAsync();
        }

        protected virtual async ValueTask<bool> OnLocationChanging()
        {
            return true;
        }

        protected virtual ValueTask ChangeLocation() => ValueTask.CompletedTask;

        public virtual void Dispose()
        {
            this._backButton.RightClick = null;
            this._backButton.IconRight = "ellipsis";

            this._cts.Cancel();


            if (this._registerLocationChangeHandler is not null)
            {
                this._registerLocationChangeHandler.Dispose();
            }

            this._cts.Dispose();
        }
    }
}

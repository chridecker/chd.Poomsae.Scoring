using Blazored.Modal.Services;
using chd.Poomsae.Scoring.App.Platforms.iOS.Authentication.Dtos;
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
using chd.UI.Base.Contracts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class AppleSignInManager : LicenseTokenProfileService
    {
        private readonly IDataService _dataService;
        private readonly FirebaseAuthService _firebaseAuthService;

        public AppleSignInManager(IModalService modalService, IDataService dataService, ISettingManager settingManager, ITokenService tokenService, FirebaseAuthService firebaseAuthService) : base(settingManager, tokenService, modalService)
        {
            this._dataService = dataService;
            this._firebaseAuthService = firebaseAuthService;
        }

        protected override async Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken)
        {
            var userToken = await this._firebaseAuthService.CurrentUserToken();
            if (string.IsNullOrWhiteSpace(userToken))
            {
                _ = await this.GetUser();
            }
            return await this._dataService.GetOrCreateDevice();
        }
        public override async Task RenewLicense(CancellationToken cancellationToken = default)
        {
            await this._firebaseAuthService.SignOutAsync();
            await base.RenewLicense(cancellationToken);
        }
        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            try
            {
                var (user, testLicense) = await this.GetUser();
                if (user is not null)
                {
                    var fsUser = await this._dataService.GetOrCreateUser(new PSUserDto()
                    {
                        Username = user.DisplayName ?? string.Empty,
                        Email = user.Email,
                        UID = user.LocalId,
                        ValidTo = testLicense ? DateTimeOffset.Now.Date.AddDays(7) : DateTimeOffset.Now.Date,
                    });

                    fsUser.UserDevice = await this._dataService.GetOrCreateUserDevice(fsUser.UID, this.Device.UID, fsUser.IsAdmin || testLicense);
                    return fsUser;
                }
            }
            catch (Exception ex)
            {
                await this._modalService.ShowDialog(ex.Message, EDialogButtons.OK);
            }
            return null;

        }
        private async Task<(FirebaseAuthDto, bool)> GetUser()
        {
            await this._firebaseAuthService.SignOutAsync();
            FirebaseAuthDto user = null;
            var testLicense = false;
            try
            {
                user = await _firebaseAuthService.SignInWithEmailAndPasswordAsync("chdscopoom@gmail.com", "ch3510ri");
                testLicense = true;
            }
            catch { }
            user ??= await this._firebaseAuthService.SignInWithAppleAsync();

            return (user, testLicense);
        }
    }
}

using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
using chd.UI.Base.Contracts.Enum;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android.Authentication
{
    public class GoogleSignInManager : LicenseTokenProfileService
    {
        private readonly IDataService _dataService;
        private readonly IFirebaseAuthGoogle _firebaseAuthGoogle;
        private readonly IFirebaseAuth _firebaseAuth;

        public GoogleSignInManager(IDataService dataService,
            IFirebaseAuthGoogle firebaseAuthGoogle, IFirebaseAuth firebaseAuth,
            IModalService modalService,
            ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService, modalService)
        {
            this._dataService = dataService;
            this._firebaseAuthGoogle = firebaseAuthGoogle;
            this._firebaseAuth = firebaseAuth;
        }

        protected override async Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken)
        {

            if (this._firebaseAuth.CurrentUser is null)
            {
                _ = await this.GetUser();
            }
            return await this._dataService.GetOrCreateDevice();
        }

        public override async Task RenewLicense(CancellationToken cancellationToken = default)
        {
            await this._firebaseAuthGoogle.SignOutAsync();
            await base.RenewLicense(cancellationToken);
        }


        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            try
            {
                await this.WaitForPermissions(cancellationToken);
                var (user, testLicense) = await this.GetUser();
                if (user is not null)
                {
                    var fsUser = await this._dataService.GetOrCreateUser(new PSUserDto()
                    {
                        Username = user.DisplayName ?? string.Empty,
                        Email = user.Email,
                        UID = user.Uid,
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

        private async Task WaitForPermissions(CancellationToken cancellationToken)
        {
            var perm = await Permissions.CheckStatusAsync<InternetPermission>();
            while (perm is not PermissionStatus.Granted)
            {
                _ = await Permissions.RequestAsync<InternetPermission>();

                perm = await Permissions.CheckStatusAsync<InternetPermission>();
                await Task.Delay(250, cancellationToken);
                if (cancellationToken.IsCancellationRequested || perm is PermissionStatus.Granted)
                {
                    break;
                }
            }
        }
        private async Task<(IFirebaseUser, bool)> GetUser()
        {
            await this._firebaseAuth.SignOutAsync();
            IFirebaseUser user = null;
            var testLicense = false;
            try
            {
                user = await _firebaseAuth.SignInWithEmailAndPasswordAsync("chdscopoom@gmail.com", "ch3510ri");
                testLicense = true;
            }
            catch { }
            user ??= await this._firebaseAuthGoogle.SignInWithGoogleAsync();

            return (user, testLicense);
        }


    }
}

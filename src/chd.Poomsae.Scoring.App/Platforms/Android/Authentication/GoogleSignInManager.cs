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
        private readonly IFirebaseAuthGoogle _firebaseAuthGoogle;
        private readonly IFirebaseAuth _firebaseAuth;

        public GoogleSignInManager(
            IFirebaseAuthGoogle firebaseAuthGoogle, IFirebaseAuth firebaseAuth,
            IModalService modalService,
            ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService, modalService)
        {
            this._firebaseAuthGoogle = firebaseAuthGoogle;
            this._firebaseAuth = firebaseAuth;
        }

        protected override async Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken)
        {
            return new PSDeviceDto()
            {
                UID = "test"
            };
        }
        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            try
            {
                await this.WaitForPermissions(cancellationToken);
                var (user, testLicense) = await this.GetUser();
                if(user is not null)
                {
                    return new PSUserDto()
                    {
                        Email = user.Email,
                        Username = user.DisplayName,
                        FirstName = " ",
                        LastName = " ",
                        UID = user.Uid,
                        HasLicense = testLicense,
                    };
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

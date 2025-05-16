using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class AppleSignInManager : LicenseTokenProfileService
    {
        private readonly IFirebaseAuth _firebaseAuth;
        private readonly IModalService _modalService;
        private readonly AppleIdSignService _appleIdSignService;

        public AppleSignInManager(IFirebaseAuth firebaseAuth, AppleIdSignService signInService,
            IModalService modalService, ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService)
        {
            this._modalService = modalService;
            this._appleIdSignService = signInService;
            this._firebaseAuth = firebaseAuth;
        }

        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            try
            {
                await this._firebaseAuth.SignOutAsync();
                IFirebaseUser user = null;
                try
                {
                    user = await _firebaseAuth.SignInWithEmailAndPasswordAsync("christoph.decker@gmx.at", "ch3510ri");
                }
                catch { }
                if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 13)
                {
                    user = await this._firebaseAuth.SignInWithAppleAsync();
                }
                else
                {
                    var idToken = await this._appleIdSignService.SignInAsync();
                    // Dann an Firebase weiterleiten:
                    var credential = OAuthProvider.GetCredential("apple.com", idToken, null, null);
                    user = await this._firebaseAuth.SignInWithCredentialAsync(credential);
                }
                if (user is not null)
                {
                    return await this._firestoreManager.GetOrCreateUser(new PSUserDto()
                    {
                        Username = user.DisplayName ?? string.Empty,
                        Email = user.Email,
                        UID = user.Uid,
                    });
                }
            }
            catch (Exception ex)
            {
                await this._modalService.ShowDialog(ex.Message, chd.UI.Base.Contracts.Enum.EDialogButtons.OK);
            }
            return null;
        }


    }
}

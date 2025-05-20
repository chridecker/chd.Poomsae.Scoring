using Blazored.Modal.Services;
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Firebase.Auth;
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
        private readonly FirestoreManager _firestoreManager;

        public AppleSignInManager(IFirebaseAuth firebaseAuth, FirestoreManager firestoreManager,
            IModalService modalService, ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService)
        {
            this._modalService = modalService;
            this._firestoreManager = firestoreManager;
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
                user = await this._firebaseAuth.SignInWithAppleAsync();
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

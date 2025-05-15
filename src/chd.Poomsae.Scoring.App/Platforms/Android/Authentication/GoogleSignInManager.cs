using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Settings;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Firebase;
using Firebase.Auth;
using Microsoft.Extensions.Options;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class GoogleSignInManager : LicenseTokenProfileService
    {
        private readonly IFirebaseAuthGoogle _firebaseAuthGoogle;
        private readonly FirestoreManager _firestoreManager;
        private readonly IModalService _modalService;

        private PSUserDto _userDto;
        private DateTime? _lastLogin;

        public GoogleSignInManager(IFirebaseAuthGoogle firebaseAuthGoogle, FirestoreManager firestoreManager, IModalService modalService,
            IOptionsMonitor<LicenseSettings> optionsMonitor) : base(optionsMonitor)
        {
             this._firebaseAuthGoogle = firebaseAuthGoogle;
            this._firestoreManager = firestoreManager;
            this._modalService = modalService;
        }

        private async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            try
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
                //var user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync("christoph.decker@gmx.at", "ch3510ri");
                var user = await this._firebaseAuthGoogle.SignInWithGoogleAsync();
                if (user != null)
                {
                    string name = user.DisplayName;
                    string email = user.Email;
                    var uid = user.Uid;

                    return await this._firestoreManager.GetOrCreateUser(new PSUserDto()
                    {
                        Username = name,
                        Email = email,
                        UID = uid,
                    });
                }
            }
            catch (Exception ex)
            {
                await this._modalService.ShowDialog(ex.Message, chd.UI.Base.Contracts.Enum.EDialogButtons.OK);
            }
            return null;
        }

        protected override async Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            if (this._userDto is not null && this._lastLogin.HasValue && this._lastLogin.Value > DateTime.Now.AddHours(-1))
            {
                return this._userDto;
            }

            this._userDto = await this.SignIn(cancellationToken);
            this._lastLogin = DateTime.Now;
            return this._userDto;
        }
    }
}

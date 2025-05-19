using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
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
        private readonly IFirebaseAuth _firebaseAuth;
        private readonly FirestoreManager _firestoreManager;
        private readonly IModalService _modalService;



        public GoogleSignInManager(IFirebaseAuthGoogle firebaseAuthGoogle, IFirebaseAuth firebaseAuth, FirestoreManager firestoreManager, IModalService modalService,
            ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService)
        {
            this._firebaseAuthGoogle = firebaseAuthGoogle;
            this._firebaseAuth = firebaseAuth;
            this._firestoreManager = firestoreManager;
            this._modalService = modalService;
        }

        protected override Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken) => this._firestoreManager.GetOrCreateDevice();


        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
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
                await this._firebaseAuth.SignOutAsync();

                IFirebaseUser user = null;
                var tetsLicense = false;
                try
                {
                    user = await _firebaseAuth.SignInWithEmailAndPasswordAsync("chdscopoom@gmail.com", "ch3510ri");
                    tetsLicense = true;
                }
                catch { }
                user ??= await this._firebaseAuthGoogle.SignInWithGoogleAsync();

                if (user is not null)
                {
                    var fsUser = await this._firestoreManager.GetOrCreateUser(new PSUserDto()
                    {
                        Username = user.DisplayName ?? string.Empty,
                        Email = user.Email,
                        UID = user.Uid,
                        ValidTo = tetsLicense ? DateTimeOffset.Now.Date.AddDays(7) : DateTimeOffset.Now.Date,
                    });

                    fsUser.UserDevice = await this._firestoreManager.GetOrCreateUserDevice(fsUser.UID, this.Device.UID, fsUser.IsAdmin || tetsLicense);
                    return fsUser;
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

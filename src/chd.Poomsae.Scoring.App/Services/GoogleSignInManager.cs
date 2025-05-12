using Blazorise;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Platforms.Android;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Firebase;
using Firebase.Auth;
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
    public class GoogleSignInManager : ProfileService<Guid, int>
    {
        private readonly IFirebaseAuthGoogle _firebaseAuthGoogle;
        private readonly FirestoreManager _firestoreManager;

        public GoogleSignInManager(IFirebaseAuthGoogle firebaseAuthGoogle, FirestoreManager firestoreManager)
        {
            this._firebaseAuthGoogle = firebaseAuthGoogle;
            this._firestoreManager = firestoreManager;
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

                //var user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync("christoph.decker@gmx.at","ch3510ri");
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

            }
            return null;
        }

        protected override async Task<UserPermissionDto<int>> GetPermissions(UserDto<Guid, int> dto, CancellationToken cancellationToken = default)
        {
            return new UserPermissionDto<int>();
        }

        protected override async Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            var user = await this.SignIn(cancellationToken);
            return user;
        }
    }
}

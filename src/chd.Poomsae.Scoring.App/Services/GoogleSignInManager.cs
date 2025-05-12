using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Firebase;
using Firebase.Auth;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class GoogleSignInManager : ProfileService<Guid, int>
    {
        public GoogleSignInManager()
        {

        }
        private async Task<PSUserDto> SignIn()
        {
            try
            {
                //var user =await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync("christoph.decker@gmx.at","ch3510ri");
                var user = await CrossFirebaseAuthGoogle.Current.SignInWithGoogleAsync();

                if (user != null)
                {
                    string name = user.DisplayName;
                    string email = user.Email;
                    string uid = user.Uid;
                    return new PSUserDto()
                    {
                        Username = name,
                        Email = email,
                        UID = uid,
                    };
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
            var user = await this.SignIn();
            return user;
        }
    }
}

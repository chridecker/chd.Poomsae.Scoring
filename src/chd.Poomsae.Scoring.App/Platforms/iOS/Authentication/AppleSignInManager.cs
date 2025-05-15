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
            IModalService modalService,
            IOptionsMonitor<LicenseSettings> optionsMonitor) : base(optionsMonitor)
        {
            this._modalService = modalService;
            this._appleIdSignService = signInService;
            this._firebaseAuth = firebaseAuth;
        }

        private async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            try
            {
                IFirebaseUser user = null;
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


    }
}

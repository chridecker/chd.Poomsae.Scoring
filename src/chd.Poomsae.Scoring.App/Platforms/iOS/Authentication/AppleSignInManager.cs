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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class AppleSignInManager : LicenseTokenProfileService
    {
        private readonly IFirebaseAuth _firebaseAuth;
        private readonly IModalService _modalService;
        private readonly IUserService _firestoreManager;

        public AppleSignInManager(IFirebaseAuth firebaseAuth, IUserService firestoreManager,
            IModalService modalService, ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService)
        {
            this._modalService = modalService;
            this._firestoreManager = firestoreManager;
            this._firebaseAuth = firebaseAuth;
        }

        protected override async Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken)
        {
            return new PSDeviceDto()
            {
                UID ="Test",
            };

            if (this._firebaseAuth.CurrentUser is null)
            {
                _ = await this.GetUser();
            }
            return await this._firestoreManager.GetOrCreateDevice();
        }
        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            return new PSUserDto()
            {
                UID = "Test",
                Email =  "Test@test.at",
                FirstName="",
                LastName="",
                HasLicense = true,
                UserDevice = new PSUserDeviceDto()
                {
                    Id ="Test",
                    Device_UID ="Test",
                    User_UID = "Test",
                    IsAllowed = true
                }
            };

            try
            {
                var (user, testLicense) = await this.GetUser();
                if (user is not null)
                {
                    var fsUser = await this._firestoreManager.GetOrCreateUser(new PSUserDto()
                    {
                        Username = user.DisplayName ?? string.Empty,
                        Email = user.Email,
                        UID = user.Uid,
                        ValidTo = testLicense ? DateTimeOffset.Now.Date.AddDays(7) : DateTimeOffset.Now.Date,
                    });

                    fsUser.UserDevice = await this._firestoreManager.GetOrCreateUserDevice(fsUser.UID, this.Device.UID, fsUser.IsAdmin || testLicense);
                    return fsUser;
                }
            }
            catch (Exception ex)
            {
                await this._modalService.ShowDialog(ex.Message, chd.UI.Base.Contracts.Enum.EDialogButtons.OK);
            }
            return null;
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
            user ??= await this._firebaseAuth.SignInWithAppleAsync();

            return (user, testLicense);
        }
    }
}

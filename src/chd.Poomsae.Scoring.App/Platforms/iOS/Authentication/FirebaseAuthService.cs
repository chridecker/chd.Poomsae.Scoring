using Blazored.Modal.Services;
using chd.Poomsae.Scoring.App.Platforms.iOS.Authentication.Dtos;
using chd.Poomsae.Scoring.App.Settings;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Enum;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class FirebaseAuthService
    {
        private const string USER = "user_token";

        private readonly IOptionsMonitor<FirebaseAuthServiceSettings> _optionsMonitor;
        private readonly IAppleSignInAuthenticator _appleSignInService;
        private readonly IModalService _modalService;

        public FirebaseAuthService(IOptionsMonitor<FirebaseAuthServiceSettings> optionsMonitor, IAppleSignInAuthenticator appleSignInService, IModalService modalService)
        {
            this._optionsMonitor = optionsMonitor;
            this._appleSignInService = appleSignInService;
            this._modalService = modalService;
        }

        public async Task<FirebaseAuthDto> SignInWithEmailAndPasswordAsync(string email, string password, bool createUserAutomatically = true)
        {
            var url = this._optionsMonitor.CurrentValue.EmailApiUrl + this._optionsMonitor.CurrentValue.ApiKey;
            return await this.HandleFirbaseCall(url, new EmailPasswordLoginDto { Email = email, Password = password });
        }

        public async Task<FirebaseAuthDto> SignInWithAppleAsync()
        {

            var appleIdSignInToken = await this._appleSignInService.AuthenticateAsync(new AppleSignInAuthenticator.Options()
            {
                IncludeEmailScope = true,
                IncludeFullNameScope = true,
            });

            var url = this._optionsMonitor.CurrentValue.AppleApiUrl + this._optionsMonitor.CurrentValue.ApiKey;

            var payload = new
            {
                postBody = $"id_token={appleIdSignInToken?.IdToken}&providerId=apple.com",
                requestUri = "http://localhost", // Muss gesetzt sein, kann Dummy sein
                returnIdpCredential = true,
                returnSecureToken = true
            };
            var user = await this.HandleFirbaseCall(url, payload);

            user.DisplayName = appleIdSignInToken.Get("user_id") +  " / " + appleIdSignInToken.Get("name");

            return user;
        }
#if DEBUG
        public async Task<string> CurrentUserToken() => Preferences.Get(USER, string.Empty);
#else
        public Task<string> CurrentUserToken() => SecureStorage.GetAsync(USER);
#endif
        public Task SignOutAsync() => this.SetToken(string.Empty);

        private async Task<FirebaseAuthDto> HandleFirbaseCall<TData>(string url, TData data)
        {
            using var client = new HttpClient();
            var res = await client.PostAsJsonAsync(url, data);
            if (res.IsSuccessStatusCode)
            {
                var user = await res.Content.ReadFromJsonAsync<FirebaseAuthDto>();
                await this.SetToken(user.IdToken);
                return user;
            }
            return null;
        }
#if DEBUG
        private async Task SetToken(string token) => Preferences.Set(USER, token);
#else
        private Task SetToken(string token) => SecureStorage.SetAsync(USER, token);
#endif
    }
}

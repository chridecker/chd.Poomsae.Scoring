using chd.Poomsae.Scoring.App.Platforms.iOS.Authentication.Dtos;
using chd.Poomsae.Scoring.App.Settings;
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

        public FirebaseAuthService(IOptionsMonitor<FirebaseAuthServiceSettings> optionsMonitor, IAppleSignInAuthenticator appleSignInService)
        {
            this._optionsMonitor = optionsMonitor;
            this._appleSignInService = appleSignInService;
        }

        public async Task<FirebaseAuthDto> SignInWithEmailAndPasswordAsync(string email, string password, bool createUserAutomatically = true)
        {
            var url = this._optionsMonitor.CurrentValue.EmailApiUrl + this._optionsMonitor.CurrentValue.ApiKey;
            return await this.HandleFirbaseCall(url, new EmailPasswordLoginDto { Email = email, Password = password });
        }

        public async Task<FirebaseAuthDto> SignInWithAppleAsync()
        {
            var appleIdSignInToken = await this._appleSignInService.AuthenticateAsync();

            var url = this._optionsMonitor.CurrentValue.AppleApiUrl + this._optionsMonitor.CurrentValue.ApiKey;

            var payload = new
            {
                postBody = $"id_token={appleIdSignInToken.IdToken}&providerId=apple.com",
                requestUri = "http://localhost", // Muss gesetzt sein, kann Dummy sein
                returnIdpCredential = true,
                returnSecureToken = true
            };
            return await this.HandleFirbaseCall(url, payload);
        }
        public Task<string> CurrentUserToken() => SecureStorage.GetAsync(USER);
        public Task SignOutAsync() => SecureStorage.SetAsync(USER, string.Empty);

        private async Task<FirebaseAuthDto> HandleFirbaseCall<TData>(string url, TData data)
        {
            using var client = new HttpClient();
            var res = await client.PostAsJsonAsync(url, data);
            if (res.IsSuccessStatusCode)
            {
                var user = await res.Content.ReadFromJsonAsync<FirebaseAuthDto>();
                await SecureStorage.SetAsync(USER, user.IdToken);
                return user;
            }
            return null;
        }
    }
}

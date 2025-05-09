using AndroidX.Activity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;

namespace chd.Poomsae.Scoring.App.Services
{
    public class GoogleSignInManager : ISignInManager
    {
        public event EventHandler<Exception> LoginFailed;
        public event EventHandler<(string, string)> LoginSucceded;

        public Task SignIn(CancellationToken cancellationToken) => this.StartGoogleSignIn(cancellationToken);

        private async Task StartGoogleSignIn(CancellationToken cancellationToken)
        {
            var clientId = "202887990694-glhr25sti3iu90c6altj9lmtpgiaa2n3.apps.googleusercontent.com";
            var redirectUri = "com.companyname.chd.poomsae.scoring.app:/oauth2redirect";
            var authUrl = new Uri($"https://accounts.google.com/o/oauth2/v2/auth" +
                            $"?client_id={clientId}" +
                            $"&redirect_uri={redirectUri}" +
                            $"&response_type=code" +
                            $"&scope=openid%20email%20profile" +
                            $"&access_type=offline");
            var callbackUrl = new Uri(redirectUri);
            try
            {
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(authUrl, callbackUrl);

                var code = authResult?.Properties["code"];
                if (code != null)
                {
                    var tokenResponse = await ExchangeCodeForToken(code, clientId, redirectUri, cancellationToken);
                    var userInfo = await GetGoogleUserInfo(tokenResponse.AccessToken, cancellationToken);
                    this.LoginSucceded?.Invoke(this, (userInfo.Email, userInfo.Name));
                    //await DisplayAlert("Erfolgreich angemeldet", $"Hallo, {userInfo.Name} ({userInfo.Email})", "OK");
                }
            }
            catch (Exception ex)
            {
                this.LoginFailed?.Invoke(this, ex);
            }
        }
        private async Task<(string AccessToken, string IdToken)> ExchangeCodeForToken(string code, string clientId, string redirectUri, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", clientId },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        });

            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            return (
                AccessToken: doc.RootElement.GetProperty("access_token").GetString(),
                IdToken: doc.RootElement.GetProperty("id_token").GetString()
            );
        }

        private async Task<(string Name, string Email)> GetGoogleUserInfo(string accessToken, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/userinfo", cancellationToken);

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            return (
                Name: root.GetProperty("name").GetString(),
                Email: root.GetProperty("email").GetString()
            );
        }
    }
}

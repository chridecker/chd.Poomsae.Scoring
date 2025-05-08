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

namespace chd.Poomsae.Scoring.App.Services
{
    public class GoogleSignInManager : ISignInManager
    {
        private IActivityResultLauncher _signInLauncher;

        public event EventHandler<Exception> LoginFailed;
        public event EventHandler<(string, string)> LoginSucceded;

        public void SetLauncher(IActivityResultLauncher signInLauncher)
        {
            this._signInLauncher = signInLauncher;
        }

        public void SignIn() => this.StartGoogleSignIn();

        public void InvokeLoginFailed(Exception ex)
        {
            this.LoginFailed?.Invoke(this, ex);
        }
        public void InvokeLoginSuccess(string email, string name)
        {
            this.LoginSucceded?.Invoke(this, (email, name));
        }

        private void StartGoogleSignIn()
        {
            if (this._signInLauncher is null) { return; }
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestEmail()
                .Build();

            var gsc = GoogleSignIn.GetClient(this, gso);
            var signInIntent = gsc.SignInIntent;
            this._signInLauncher.Launch(signInIntent);
        }
    }
}

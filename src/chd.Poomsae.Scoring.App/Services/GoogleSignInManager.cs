using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class GoogleSignInManager
    {
        public GoogleSignInManager()
        {

        }
        public async Task SignIn()
        {
            var user = await CrossFirebaseAuthGoogle.Current.SignInWithGoogleAsync();

            if (user != null)
            {
                string name = user.DisplayName;
                string email = user.Email;
                string uid = user.Uid;

                // Hier hast du Zugriff auf den aktuell angemeldeten Google-Nutzer
            }
        }
    }
}

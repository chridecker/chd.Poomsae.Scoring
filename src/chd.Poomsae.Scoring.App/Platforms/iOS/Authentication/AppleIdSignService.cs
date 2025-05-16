using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chd.Poomsae.Scoring.UI.Services;

[assembly: Dependency(typeof(AppleSignInService))]
namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class AppleIdSignService : NSObject
    {
        private TaskCompletionSource<string> _tcs;
        public Task<string> SignInAsync()
        {
            _tcs = new TaskCompletionSource<string>();

            var provider = new ASAuthorizationAppleIdProvider();
            var request = provider.CreateRequest();
            request.RequestedScopes = new[] { ASAuthorizationScope.Email, ASAuthorizationScope.FullName };

            var controller = new ASAuthorizationController(new[] { request });
            controller.Delegate = this;
            controller.PerformRequests();

            return _tcs.Task;
        }
        [Export("authorizationController:didCompleteWithAuthorization:")]
        public void DidComplete(ASAuthorizationController controller, ASAuthorization authorization)
        {
            if (authorization.GetCredential() is ASAuthorizationAppleIdCredential credential)
            {
                var idToken = NSString.FromData(credential.IdentityToken, NSStringEncoding.UTF8);
                _tcs.TrySetResult(idToken);
            }
        }

        [Export("authorizationController:didCompleteWithError:")]
        public void DidComplete(ASAuthorizationController controller, NSError error)
        {
            _tcs.TrySetException(new NSErrorException(error));
        }
    }
}

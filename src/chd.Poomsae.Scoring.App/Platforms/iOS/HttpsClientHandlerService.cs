using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public static class HttpsClientHandlerService
    {
        public static HttpMessageHandler GetPlatformMessageHandler() => new NSUrlSessionHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            TrustOverrideForUrl = (sender, url, trust) => true,
        };

    }
}

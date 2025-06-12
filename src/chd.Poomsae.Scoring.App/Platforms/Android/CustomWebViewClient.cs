using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public class CustomWebViewClient : WebViewClient
    {
        private readonly Action<global::Android.Webkit.WebView?, string?> _handler;

        public CustomWebViewClient(Action<global::Android.Webkit.WebView?, string?> handler)
        {
            this._handler = handler;
        }

        public override void OnPageFinished(global::Android.Webkit.WebView? view, string? url)
        {
            base.OnPageFinished(view, url);
            this._handler?.Invoke(view, url);
        }
    }
}

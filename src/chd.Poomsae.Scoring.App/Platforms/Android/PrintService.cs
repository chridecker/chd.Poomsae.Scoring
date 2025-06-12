using Android.Content;
using Android.Print;
using Android.Webkit;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WV = Android.Webkit.WebView;

namespace chd.Poomsae.Scoring.App.Platforms.Android
{
    public class PrintService : BasePrintService
    {
        public PrintService(HtmlRenderer htmlRenderer) : base(htmlRenderer)
        {
        }

        public override Task PrintHtmlAsync(string html, string? jobName = null)
        {
            var activity = Platform.CurrentActivity!;

            var webView = new WV(activity);
            webView.LoadDataWithBaseURL(null, html, "text/HTML", "UTF-8", null);

            var tcs = new TaskCompletionSource();

            var client = new CustomWebViewClient((view, url) =>
                {
                    var printMgr = (PrintManager)activity.GetSystemService(Context.PrintService)!;
                    var printAdapter = webView.CreatePrintDocumentAdapter(jobName ?? "My Document");
                    printMgr.Print(jobName ?? "PrintJob", printAdapter, null);
                    tcs.TrySetResult();
                });
            webView.SetWebViewClient(client);

            return tcs.Task;
        }
    }
}

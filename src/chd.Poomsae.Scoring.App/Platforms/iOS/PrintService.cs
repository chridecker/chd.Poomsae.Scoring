using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public class PrintService : BasePrintService
    {
        public PrintService(HtmlRenderer htmlRenderer) : base(htmlRenderer)
        {
        }
        public override Task PrintHtmlAsync(string html, string? jobName = null)
        {
            var printFormatter = new UIMarkupTextPrintFormatter(html);
            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.JobName = jobName ?? "My Document";
            printInfo.OutputType = UIPrintInfoOutputType.General;

            var printController = UIPrintInteractionController.SharedPrintController;
            printController.PrintFormatter = printFormatter;
            printController.PrintInfo = printInfo;

            var tcs = new TaskCompletionSource();
            printController.Present(true, (controller, completed, err) =>
            {
                tcs.TrySetResult();
            });

            return tcs.Task;
        }
    }
}

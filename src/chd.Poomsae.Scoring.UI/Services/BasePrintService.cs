using chd.Poomsae.Scoring.Contracts.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public abstract class BasePrintService : IPrintService
    {
        private readonly HtmlRenderer _htmlRenderer;

        protected BasePrintService(HtmlRenderer htmlRenderer)
        {
            this._htmlRenderer = htmlRenderer;
        }
        public async Task PrintComponentAsync<TComponent>(IDictionary<string, object?> paramDict = null, string? jobName = null) where TComponent : ComponentBase
        {
            var html = await this._htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var parameters = ParameterView.FromDictionary(paramDict);
                var output = await this._htmlRenderer.RenderComponentAsync<TComponent>(parameters);

                return output.ToHtmlString();
            });
            await this.PrintHtmlAsync(html, jobName);
        }

        public abstract Task PrintHtmlAsync(string html, string? jobName = null);
    }
}

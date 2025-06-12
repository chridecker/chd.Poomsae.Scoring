using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IPrintService
    {
        Task PrintHtmlAsync(string html, string? jobName = null);
        Task PrintComponentAsync<TComponent>(IDictionary<string, object?> paramDict, string? jobName = null)
            where TComponent : ComponentBase;
    }
}

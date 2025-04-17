using chd.UI.Base.Client.Implementations.Services.Base;
using chd.UI.Base.Contracts.Interfaces.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class UIComponentHandler : BaseUIComponentHandler
    {
        public UIComponentHandler(IJSRuntime jSRuntime, IDefaultIconService defaultIconService) : base(jSRuntime, defaultIconService)
        {
        }
    }
}

using Blazored.Modal;
using Blazored.Modal.Services;
using chd.Poomsae.Scoring.UI.Components.Shared;
using chd.UI.Base.Components.General.Form;
using chd.UI.Base.Components.General.Loading;
using chd.UI.Base.Contracts.Enum;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Extensions
{
    public static class ModalExtensions
    {
        public static IModalReference ShowLoading(this IModalService modalService, string message = "")
        {
            ModalParameters modalParameters = new ModalParameters();
            modalParameters.Add(nameof(Loading.Show), true);
            modalParameters.Add(nameof(Loading.Message), message);
            return modalService.Show<Loading>(null, modalParameters, new ModalOptions
            {
                HideHeader = true,
                DisableBackgroundCancel = true,
                HideCloseButton = true,
                Class = "chd-app-modal-loading"
            });
        }

        public static async Task<EDialogResult> ShowSmallDialog(this IModalService modalService, string message, EDialogButtons buttons, RenderFragment? childContent = null)
        {
            var parameter = new ModalParameters()
            {
                {nameof(MessageDialog.Buttons),buttons},
                {nameof(MessageDialog.ChildContent),childContent }
            };
            var modal = modalService.Show<MessageDialog>(message, parameter, new ModalOptions
            {
                DisableBackgroundCancel = true,
                HideCloseButton = true,
                Class = "chd-app-modal-small"
            });
            ModalResult res = await modal.Result;
            if (res.Cancelled)
            {
                return EDialogResult.None;
            }

            object data = res.Data;
            EDialogResult dialogResult = default(EDialogResult);
            int num;
            if (data is EDialogResult)
            {
                dialogResult = (EDialogResult)data;
                num = 1;
            }
            else
            {
                num = 0;
            }

            if (num == 0)
            {
                throw new Exception($"Ergebnis von {"MessageDialog"} ist ungültig [{res.Cancelled}, {res.Data}]");
            }

            return dialogResult;
        }

        public static Task<EDialogResult> ShowYesNoDialog(this IModalService modalService, string message, RenderFragment? childContent = null)
        => modalService.ShowSmallDialog(message, EDialogButtons.YesNo, childContent);
    }
}

﻿using Microsoft.AspNetCore.Components;
using Radzen;

namespace bbt.service.notification.ui.Component.Modal
{
    public partial class BaseModal: BaseModalCode
    {
        [Inject]
        DialogService DialogService { get; set; }

        protected override void OpenModal()
        {
            DialogOptions dialogOptions = new DialogOptions();
            dialogOptions.ShowClose = ShowClose;
            dialogOptions.ShowTitle = ShowTitle;


            if (IsFullScreen)
            {
                dialogOptions.Width = "90%";
                dialogOptions.Height = "90%";
            }
            else
            {
                if (!string.IsNullOrEmpty(Width))
                    dialogOptions.Width = Width;

                if (!string.IsNullOrEmpty(Height))
                    dialogOptions.Width = Height;

                if (!string.IsNullOrEmpty(Style))
                    dialogOptions.Style = Style;
            }

            DialogService.Open(Title, ds => ChildContent, dialogOptions);
            //DialogService.OnClose += DialogService_OnClose;
        }

        //private void DialogService_OnClose(dynamic obj)
        //{
        //    OnClose?.Invoke(ModalName);
        //    DialogService.OnClose -= DialogService_OnClose;

        //}

        protected override void CloseModal()
        {
            DialogService.Close();
            OnClose?.Invoke(ModalName);

        }
    }
}

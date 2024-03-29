﻿using Microsoft.AspNetCore.Components;
using System;

namespace bbt.service.notification.ui.Component.Modal
{
    public class BaseModalCode :BaseComponent
    {
        #region content
        [Parameter]
        public bool ShowTitle { get; set; } = true;
        [Parameter]
        public bool ShowClose { get; set; } = true;

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public RenderFragment FooterContent { get; set; }
        #endregion


        [Parameter]
        public string ModalName { get; set; }
        [Parameter]
        public Action<string> OnClose { get; set; }


        public bool IsModalOpen = false;

        #region settings
        [Parameter]
        public bool IsFullScreen { get; set; } = false;

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public string Height { get; set; }
        #endregion

        public virtual void Open()
        {
            IsModalOpen = true;
            OpenModal();
        }
        protected virtual void OpenModal()
        {

        }
        protected virtual void CloseModal()
        {

        }

        public virtual void Close()
        {
            IsModalOpen = false;
            CloseModal();
        }
    }
}

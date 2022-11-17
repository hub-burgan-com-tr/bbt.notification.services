﻿namespace bbt.service.notification.ui.Component
{
    public interface INotification
    {
        void Clear();

        void ShowErrorMessage(string title, string message);

        void ShowInfoMessage(string title, string message);

        void ShowSuccessMessage(string title, string message);

        void ShowWarningMessage(string title, string message);
    }
}

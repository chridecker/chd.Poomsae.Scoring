using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public abstract class BaseNotificationManager : INotificationManagerService
    {
        protected int _messageId = 0;

        public const string DataKey = "data";
        public const string DataTypeKey = "datatype";

        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public abstract void SendNotification(string title, string message, bool autoCloseOnLick = true);
        public abstract void SendNotification<TData>(string title, string message, TData data, bool autoCloseOnLick = true);
        public abstract void ReceiveNotification(NotificationEventArgs args);

        protected void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(this, e);
        }
    }
}

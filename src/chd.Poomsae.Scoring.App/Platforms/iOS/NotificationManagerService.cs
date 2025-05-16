using chd.Poomsae.Scoring.Contracts.Interfaces;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserNotifications;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public class NotificationManagerService : INotificationManagerService
    {
        private int messageId = 0;
        private bool hasNotificationsPermission;

        public const string DataKey = "data";
        public const string DataTypeKey = "datatype";

        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public NotificationManagerService(NotificationReceiver receiver)
        {
            // Create a UNUserNotificationCenterDelegate to handle incoming messages.
            UNUserNotificationCenter.Current.Delegate = receiver;

            // Request permission to use local notifications.
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                hasNotificationsPermission = approved;
            });
        }
        public void SendNotification(string title, string message, bool autoCloseOnLick = true)
        {
            if (!hasNotificationsPermission) { return; }
            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Subtitle = "",
                Body = message,
                Badge = 1
            };
            this.Show(content);
        }

        public void SendNotification<TData>(string title, string message, TData data, bool autoCloseOnLick = true)
        {
            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Subtitle = "",
                Body = message,
                Badge = 1
            };
            content.UserInfo = NSDictionary.FromObjectsAndKeys(
                [new NSString(typeof(TData).FullName), new NSString(JsonSerializer.Serialize(data))],
                [new NSString(DataTypeKey), new NSString(DataKey)]);
            this.Show(content);
        }

        private void Show(UNMutableNotificationContent content)
        {
            messageId++;
            DateTime? notifyTime = null;
            UNNotificationTrigger trigger;
            if (notifyTime.HasValue)
                // Create a calendar-based trigger.
                trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponents(notifyTime.Value), false);
            else
                // Create a time-based trigger, interval is in seconds and must be greater than 0.
                trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.25, false);

            var request = UNNotificationRequest.FromIdentifier(messageId.ToString(), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                    throw new Exception($"Failed to schedule notification: {err}");
            });
        }

        public void ReceiveNotification(NotificationEventArgs args) => NotificationReceived?.Invoke(this, args);


        NSDateComponents GetNSDateComponents(DateTime dateTime)
        {
            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }
    }
}

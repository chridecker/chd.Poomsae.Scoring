using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNotifications;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{
    public class NotificationReceiver : UNUserNotificationCenterDelegate
    {
        // Called if app is in the foreground.
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            ProcessNotification(notification);

            var presentationOptions = (OperatingSystem.IsIOSVersionAtLeast(14))
                ? UNNotificationPresentationOptions.Banner
                : UNNotificationPresentationOptions.Alert;

            completionHandler(presentationOptions);
        }

        // Called if app is in the background, or killed state.
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            if (response.IsDefaultAction)
            {
                ProcessNotification(response.Notification);
            }
            completionHandler();
        }

        void ProcessNotification(UNNotification notification)
        {
            var request = notification.Request;

            string title = request.Content.Title;
            string message = request.Content.Body;
            int.TryParse(request.Identifier, out int id);
            object requestData = null;

            if (!string.IsNullOrEmpty(request.Content.UserInfo[NotificationManagerService.DataKey])
                && !string.IsNullOrEmpty(request.Content.UserInfo[NotificationManagerService.DataTypeKey]))
            {
                string type = request.Content.UserInfo[NotificationManagerService.DataTypeKey];
                string data = request.Content.UserInfo[NotificationManagerService.DataKey];

                var t = Type.GetType(type);
                requestData = JsonSerializer.Deserialize(data, t);
            }

            var service = IPlatformApplication.Current?.Services.GetService<INotificationManagerService>();
            service?.ReceiveNotification(new NotificationEventArgs(id, title, message, data, false));
        }
    }
}

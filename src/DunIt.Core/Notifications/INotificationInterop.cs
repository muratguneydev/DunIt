namespace DunIt.Core.Notifications;

public interface INotificationInterop
{
    ValueTask<NotificationPermission> RequestPermission();
}

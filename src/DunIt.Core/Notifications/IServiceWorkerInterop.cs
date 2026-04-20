namespace DunIt.Core.Notifications;

public interface IServiceWorkerInterop
{
    ValueTask ShowNotification(string title, string body);
    ValueTask OpenApp();
}

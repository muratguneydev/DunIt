namespace DunIt.Core.Notifications;

using System.Text.Json;
using DunIt.Core.Models;

public class PushMessageHandler
{
    private readonly IServiceWorkerInterop _interop;

    public PushMessageHandler(IServiceWorkerInterop interop)
    {
        _interop = interop;
    }

    public async Task HandlePush(string payloadJson)
    {
        var payload = JsonSerializer.Deserialize<PushPayload>(payloadJson)!;
        await _interop.ShowNotification(payload.Title, payload.Body);
    }

    public async Task HandleNotificationClick()
    {
        await _interop.OpenApp();
    }
}

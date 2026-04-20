namespace DunIt.Web.Notifications;

using DunIt.Core.Notifications;
using DunIt.Web.Firebase;
using Microsoft.JSInterop;

public sealed class JsFcmInterop(IJSRuntime js, IFirebaseAppSettings settings) : IFcmInterop
{
    public async ValueTask<string> GetToken()
    {
        return await js.InvokeAsync<string>("fcm_interop.getToken", settings.VapidKey);
    }

    public async ValueTask SendTestMessage(string token)
    {
        await js.InvokeVoidAsync("fcm_interop.sendTestMessage", token, settings.FcmServerKey);
    }
}

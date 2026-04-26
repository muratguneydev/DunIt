namespace DunIt.Web.Notifications;

using DunIt.Core.Notifications;
using Microsoft.JSInterop;

public sealed class JsLocalStorage(IJSRuntime js) : ILocalStorage
{
    public async ValueTask SetItemAsync(string key, string value) =>
        await js.InvokeVoidAsync("localStorage.setItem", key, value);

    public async ValueTask<string?> GetItemAsync(string key) =>
        await js.InvokeAsync<string?>("localStorage.getItem", key);
}

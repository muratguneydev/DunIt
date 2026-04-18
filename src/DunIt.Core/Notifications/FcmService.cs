namespace DunIt.Core.Notifications;

public class FcmService
{
    private readonly IFcmInterop _fcmInterop;
    private readonly ILocalStorage _localStorage;

    public FcmService(IFcmInterop fcmInterop, ILocalStorage localStorage)
    {
        _fcmInterop = fcmInterop;
        _localStorage = localStorage;
    }

    public async Task<string> GetToken()
    {
        var token = await _fcmInterop.GetToken();
        await _localStorage.SetItemAsync("fcm_token", token);
        return token;
    }

    public async Task<string?> GetStoredToken()
    {
        return await _localStorage.GetItemAsync("fcm_token");
    }
}

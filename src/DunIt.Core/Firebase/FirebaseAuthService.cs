namespace DunIt.Core.Firebase;

using DunIt.Core.Auth;
using DunIt.Core.Models;

public sealed class FirebaseAuthService(IFirebaseInterop interop) : IAuthService
{
    public bool IsAuthenticated { get; private set; }
    public bool IsParent { get; private set; }
    public event Action AuthStateChanged = delegate { };

    public async Task RestoreSession()
    {
        IsAuthenticated = await interop.HasCurrentUser();
        if (IsAuthenticated)
        {
            var uid = await interop.GetCurrentUserId();
            IsParent = await interop.IsParent(uid);
        }
        AuthStateChanged();
    }

    public async Task<bool> SignIn()
    {
        try
        {
            await interop.SignIn();
            var uid = await interop.GetCurrentUserId();
            IsAuthenticated = true;
            IsParent = await interop.IsParent(uid);
            AuthStateChanged();
            return true;
        }
        catch
        {
            IsAuthenticated = false;
            IsParent = false;
            AuthStateChanged();
            return false;
        }
    }

    public async Task SignOut()
    {
        await interop.SignOut();
        IsAuthenticated = false;
        IsParent = false;
        AuthStateChanged();
    }
}

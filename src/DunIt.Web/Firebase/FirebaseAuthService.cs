namespace DunIt.Web.Firebase;

using DunIt.Core.Auth;
using DunIt.Core.Firebase;

public sealed class FirebaseAuthService(IFirebaseInterop interop) : IAuthService
{
    public bool IsAuthenticated { get; private set; }
    public event Action AuthStateChanged = delegate { };

    public async Task RestoreSession()
    {
        IsAuthenticated = await interop.HasCurrentUser();
        AuthStateChanged();
    }

    public async Task<bool> SignIn(Credentials credentials)
    {
        try
        {
            await interop.SignIn(credentials);
            IsAuthenticated = true;
            AuthStateChanged();
            return true;
        }
        catch
        {
            IsAuthenticated = false;
            AuthStateChanged();
            return false;
        }
    }

    public async Task SignOut()
    {
        await interop.SignOut();
        IsAuthenticated = false;
        AuthStateChanged();
    }
}

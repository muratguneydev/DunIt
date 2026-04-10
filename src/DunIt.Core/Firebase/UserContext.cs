namespace DunIt.Core.Firebase;

using DunIt.Core.Auth;
using DunIt.Core.Models;

public sealed class UserContext(IFirebaseInterop interop) : IUserContext
{
    public bool IsAuthenticated { get; private set; }
    public bool IsParent { get; private set; }
    public FirebaseUid CurrentUserId { get; private set; }
    public event Action Changed = delegate { };

    public async Task RestoreSession()
    {
        IsAuthenticated = await interop.HasCurrentUser();
        if (IsAuthenticated)
        {
            CurrentUserId = await interop.GetCurrentUserId();
            IsParent = await interop.IsParent(CurrentUserId);
        }
        Changed();
    }

    public async Task<bool> SignIn()
    {
        try
        {
            await interop.SignIn();
            CurrentUserId = await interop.GetCurrentUserId();
            IsAuthenticated = true;
            IsParent = await interop.IsParent(CurrentUserId);
            Changed();
            return true;
        }
        catch
        {
            IsAuthenticated = false;
            IsParent = false;
            CurrentUserId = default;
            Changed();
            return false;
        }
    }

    public async Task SignOut()
    {
        await interop.SignOut();
        IsAuthenticated = false;
        IsParent = false;
        CurrentUserId = default;
        Changed();
    }
}

namespace DunIt.Core.Auth;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    bool IsParent { get; }
    event Action AuthStateChanged;
    Task RestoreSession();
    Task<bool> SignIn();
    Task SignOut();
}

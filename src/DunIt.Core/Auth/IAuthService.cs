namespace DunIt.Core.Auth;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    event Action AuthStateChanged;
    Task RestoreSession();
    Task<bool> SignIn(Credentials credentials);
    Task SignOut();
}

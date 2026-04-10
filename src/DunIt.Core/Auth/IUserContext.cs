namespace DunIt.Core.Auth;

using DunIt.Core.Models;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    bool IsParent { get; }
    FirebaseUid CurrentUserId { get; }
    event Action Changed;
    Task RestoreSession();
    Task<bool> SignIn();
    Task SignOut();
}

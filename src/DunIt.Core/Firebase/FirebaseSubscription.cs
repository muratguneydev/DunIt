namespace DunIt.Core.Firebase;

using DunIt.Core.Repositories;

internal sealed class FirebaseSubscription(string id, IFirebaseInterop interop) : ISubscription
{
    public async ValueTask DisposeAsync() => await interop.Unsubscribe(id);
}

namespace DunIt.Core.Repositories;

public interface ISubscription : IAsyncDisposable { }

internal sealed class NullSubscription : ISubscription
{
    public static readonly NullSubscription Instance = new();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

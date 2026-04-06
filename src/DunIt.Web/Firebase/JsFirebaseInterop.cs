namespace DunIt.Web.Firebase;

using DunIt.Core.Firebase;
using Microsoft.JSInterop;

public sealed class JsFirebaseInterop(IJSRuntime js, FirebaseConfig config) : IFirebaseInterop, IAsyncDisposable
{
    private bool _initialized;
    private readonly Dictionary<string, IDisposable> _refs = new();

    private async Task EnsureInitialized()
    {
        if (_initialized) return;
        await js.InvokeVoidAsync("firebase_interop.init", config);
        _initialized = true;
    }

    // ── Children ────────────────────────────────────────────────────────────

    public async Task<ChildDto[]> GetChildren()
    {
        await EnsureInitialized();
        return await js.InvokeAsync<ChildDto[]>("firebase_interop.getChildren");
    }

    public async Task<ChildDto> AddChild(ChildDto child)
    {
        await EnsureInitialized();
        return await js.InvokeAsync<ChildDto>("firebase_interop.addChild", child);
    }

    public async Task DeleteChild(string childId)
    {
        await EnsureInitialized();
        await js.InvokeVoidAsync("firebase_interop.deleteChild", childId);
    }

    // ── Chores ──────────────────────────────────────────────────────────────

    public async Task<ChoreDto[]> GetChoresForChild(string childId)
    {
        await EnsureInitialized();
        return await js.InvokeAsync<ChoreDto[]>("firebase_interop.getChoresForChild", childId);
    }

    public async Task<ChoreDto> AddChore(ChoreDto chore)
    {
        await EnsureInitialized();
        return await js.InvokeAsync<ChoreDto>("firebase_interop.addChore", chore);
    }

    public async Task DeleteChore(string choreId)
    {
        await EnsureInitialized();
        await js.InvokeVoidAsync("firebase_interop.deleteChore", choreId);
    }

    // ── Completions ─────────────────────────────────────────────────────────

    public async Task<ChoreCompletionDto[]> GetCompletionsFor(string childId, string date)
    {
        await EnsureInitialized();
        return await js.InvokeAsync<ChoreCompletionDto[]>("firebase_interop.getCompletionsFor", childId, date);
    }

    public async Task<ChoreCompletionDto> CompleteChore(ChoreCompletionDto completion)
    {
        await EnsureInitialized();
        return await js.InvokeAsync<ChoreCompletionDto>("firebase_interop.completeChore", completion);
    }

    public async Task UndoChore(string completionId)
    {
        await EnsureInitialized();
        await js.InvokeVoidAsync("firebase_interop.undoChore", completionId);
    }

    // ── Real-time subscriptions ──────────────────────────────────────────────

    public async Task<string> SubscribeToChildren(Func<ChildDto[], Task> onUpdate)
    {
        await EnsureInitialized();
        var id = Guid.NewGuid().ToString();
        var dotNetRef = DotNetObjectReference.Create(new ChildSubscriptionHandler(onUpdate));
        _refs[id] = dotNetRef;
        await js.InvokeVoidAsync("firebase_interop.subscribeToChildren", id, dotNetRef);
        return id;
    }

    public async Task<string> SubscribeToChores(string childId, Func<ChoreDto[], Task> onUpdate)
    {
        await EnsureInitialized();
        var id = Guid.NewGuid().ToString();
        var dotNetRef = DotNetObjectReference.Create(new ChoreSubscriptionHandler(onUpdate));
        _refs[id] = dotNetRef;
        await js.InvokeVoidAsync("firebase_interop.subscribeToChores", childId, id, dotNetRef);
        return id;
    }

    public async Task<string> SubscribeToCompletions(string childId, string date, Func<ChoreCompletionDto[], Task> onUpdate)
    {
        await EnsureInitialized();
        var id = Guid.NewGuid().ToString();
        var dotNetRef = DotNetObjectReference.Create(new CompletionSubscriptionHandler(onUpdate));
        _refs[id] = dotNetRef;
        await js.InvokeVoidAsync("firebase_interop.subscribeToCompletions", childId, date, id, dotNetRef);
        return id;
    }

    public async Task Unsubscribe(string subscriptionId)
    {
        if (string.IsNullOrEmpty(subscriptionId)) return;
        await js.InvokeVoidAsync("firebase_interop.unsubscribe", subscriptionId);
        if (_refs.TryGetValue(subscriptionId, out var dotNetRef))
        {
            dotNetRef.Dispose();
            _refs.Remove(subscriptionId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var id in _refs.Keys.ToList())
            await js.InvokeVoidAsync("firebase_interop.unsubscribe", id);
        foreach (var dotNetRef in _refs.Values)
            dotNetRef.Dispose();
        _refs.Clear();
    }

    private class ChildSubscriptionHandler(Func<ChildDto[], Task> callback)
    {
        [JSInvokable] public Task OnDataChanged(ChildDto[] children) => callback(children);
    }

    private class ChoreSubscriptionHandler(Func<ChoreDto[], Task> callback)
    {
        [JSInvokable] public Task OnDataChanged(ChoreDto[] chores) => callback(chores);
    }

    private class CompletionSubscriptionHandler(Func<ChoreCompletionDto[], Task> callback)
    {
        [JSInvokable] public Task OnDataChanged(ChoreCompletionDto[] completions) => callback(completions);
    }
}

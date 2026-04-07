namespace DunIt.Core.Firebase;

public interface IFirebaseInterop
{
    // Children
    Task<ChildDto[]> GetChildren();
    Task<ChildDto> AddChild(ChildDto child);
    Task DeleteChild(string childId);

    // Chores
    Task<ChoreDto[]> GetChoresForChild(string childId);
    Task<ChoreDto> AddChore(ChoreDto chore);
    Task DeleteChore(string choreId);

    // Completions
    Task<ChoreCompletionDto[]> GetCompletionsFor(string childId, string date);
    Task<ChoreCompletionDto> CompleteChore(ChoreCompletionDto completion);
    Task UndoChore(string completionId);

    // Auth
    Task SignIn(string email, string password);
    Task SignOut();
    Task<bool> HasCurrentUser();

    // Real-time subscriptions — return a subscriptionId for later cancellation
    Task<string> SubscribeToChildren(Func<ChildDto[], Task> onUpdate);
    Task<string> SubscribeToChores(string childId, Func<ChoreDto[], Task> onUpdate);
    Task<string> SubscribeToCompletions(string childId, string date, Func<ChoreCompletionDto[], Task> onUpdate);
    Task Unsubscribe(string subscriptionId);
}

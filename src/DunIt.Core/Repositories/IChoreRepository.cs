namespace DunIt.Core.Repositories;

using DunIt.Core.Models;

public interface IChoreRepository
{
    Task<Chore> AddChore(Chore chore);
    Task DeleteChore(ChoreId choreId);
    Task<IReadOnlyList<Chore>> GetChoresForChild(ChildId childId);
    Task<ChoreCompletion> CompleteChore(ChoreId choreId, ChildId childId, DateTimeOffset completedAt);
    Task UndoChore(ChoreCompletionId completionId);
    Task<IReadOnlyList<ChoreCompletion>> GetCompletionsFor(ChildId childId, DateTimeOffset date);
    Task<ISubscription> SubscribeToChores(ChildId childId, Action<IReadOnlyList<Chore>> onUpdate);
    Task<ISubscription> SubscribeToCompletions(ChildId childId, DateTimeOffset date, Action<IReadOnlyList<ChoreCompletion>> onUpdate);
}

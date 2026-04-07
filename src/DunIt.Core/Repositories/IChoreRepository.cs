namespace DunIt.Core.Repositories;

using DunIt.Core.Models;

public interface IChoreRepository
{
    Task<Chore> AddChore(Chore chore);
    Task DeleteChore(string choreId);
    Task<IReadOnlyList<Chore>> GetChoresForChild(string childId);
    Task<ChoreCompletion> CompleteChore(string choreId, string childId, DateTimeOffset completedAt);
    Task UndoChore(string completionId);
    Task<IReadOnlyList<ChoreCompletion>> GetCompletionsFor(string childId, DateTimeOffset date);
    Task<ISubscription> SubscribeToChores(string childId, Action<IReadOnlyList<Chore>> onUpdate);
    Task<ISubscription> SubscribeToCompletions(string childId, DateTimeOffset date, Action<IReadOnlyList<ChoreCompletion>> onUpdate);
}

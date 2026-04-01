namespace DunIt.Core.Repositories;

using DunIt.Core.Models;

public class InMemoryChoreRepository : IChoreRepository
{
    private readonly List<Chore> _chores = [];
    private readonly List<ChoreCompletion> _completions = [];

    public Task<Chore> AddChore(Chore chore)
    {
        _chores.Add(chore);
        return Task.FromResult(chore);
    }

    public Task DeleteChore(string choreId)
    {
        _chores.RemoveAll(c => c.Id == choreId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Chore>> GetChoresForChild(string childId) =>
        Task.FromResult<IReadOnlyList<Chore>>(_chores.Where(c => c.AssignedTo == childId).ToList());

    public Task CompleteChore(string choreId, string childId, DateTimeOffset completedAt)
    {
        _completions.Add(new ChoreCompletion(Guid.NewGuid().ToString(), choreId, childId, completedAt));
        return Task.CompletedTask;
    }

    public Task UndoChore(string completionId)
    {
        _completions.RemoveAll(c => c.Id == completionId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ChoreCompletion>> GetCompletionsFor(string childId, DateTimeOffset date)
    {
        var results = _completions
            .Where(c => c.ChildId == childId && c.CompletedAt.Date == date.Date)
            .ToList();
        return Task.FromResult<IReadOnlyList<ChoreCompletion>>(results);
    }
}

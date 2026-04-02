namespace DunIt.Core.Repositories;

using DunIt.Core.Models;
using Microsoft.Extensions.Logging;

public class InMemoryChoreRepository : IChoreRepository
{
    private readonly ILogger<InMemoryChoreRepository> _logger;
    private readonly List<Chore> _chores = [];
    private readonly List<ChoreCompletion> _completions = [];

    public InMemoryChoreRepository(ILogger<InMemoryChoreRepository> logger)
    {
        _logger = logger;
    }

    public Task<Chore> AddChore(Chore chore)
    {
        try
        {
            _chores.Add(chore);
            return Task.FromResult(chore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add chore {ChoreId}", chore.Id);
            throw;
        }
    }

    public Task DeleteChore(string choreId)
    {
        try
        {
            _chores.RemoveAll(c => c.Id == choreId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete chore {ChoreId}", choreId);
            throw;
        }
    }

    public Task<IReadOnlyList<Chore>> GetChoresForChild(string childId)
    {
        try
        {
            return Task.FromResult<IReadOnlyList<Chore>>(_chores.Where(c => c.AssignedTo == childId).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get chores for child {ChildId}", childId);
            throw;
        }
    }

    public Task<ChoreCompletion> CompleteChore(string choreId, string childId, DateTimeOffset completedAt)
    {
        try
        {
            var completion = new ChoreCompletion(Guid.NewGuid().ToString(), choreId, childId, completedAt);
            _completions.Add(completion);
            return Task.FromResult(completion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete chore {ChoreId} for child {ChildId}", choreId, childId);
            throw;
        }
    }

    public Task UndoChore(string completionId)
    {
        try
        {
            _completions.RemoveAll(c => c.Id == completionId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to undo chore completion {CompletionId}", completionId);
            throw;
        }
    }

    public Task<IReadOnlyList<ChoreCompletion>> GetCompletionsFor(string childId, DateTimeOffset date)
    {
        try
        {
            var results = _completions
                .Where(c => c.ChildId == childId && c.CompletedAt.Date == date.Date)
                .ToList();
            return Task.FromResult<IReadOnlyList<ChoreCompletion>>(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get completions for child {ChildId}", childId);
            throw;
        }
    }
}

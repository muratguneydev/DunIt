namespace DunIt.Core.Repositories;

using DunIt.Core.Models;
using Microsoft.Extensions.Logging;

public class InMemoryChildRepository : IChildRepository
{
    private readonly ILogger<InMemoryChildRepository> _logger;
    private readonly List<Child> _children = [];

    public InMemoryChildRepository(ILogger<InMemoryChildRepository> logger)
    {
        _logger = logger;
    }

    public Task<Child> AddChild(Child child)
    {
        try
        {
            _children.Add(child);
            return Task.FromResult(child);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add child {ChildId}", child.Id);
            throw;
        }
    }

    public Task DeleteChild(string childId)
    {
        try
        {
            _children.RemoveAll(c => c.Id == childId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete child {ChildId}", childId);
            throw;
        }
    }

    public Task<IReadOnlyList<Child>> GetChildren()
    {
        try
        {
            return Task.FromResult<IReadOnlyList<Child>>(_children.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get children");
            throw;
        }
    }
}

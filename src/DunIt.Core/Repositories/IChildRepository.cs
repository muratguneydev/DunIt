namespace DunIt.Core.Repositories;

using DunIt.Core.Models;

public interface IChildRepository
{
    Task<Child> AddChild(Child child);
    Task DeleteChild(string childId);
    Task<IReadOnlyList<Child>> GetChildren();
}

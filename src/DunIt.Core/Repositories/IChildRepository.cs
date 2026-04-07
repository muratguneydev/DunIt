namespace DunIt.Core.Repositories;

using DunIt.Core.Models;

public interface IChildRepository
{
    Task<Child> AddChild(Child child);
    Task DeleteChild(ChildId childId);
    Task<IReadOnlyList<Child>> GetChildren();
    Task<ISubscription> Subscribe(Action<IReadOnlyList<Child>> onUpdate);
}

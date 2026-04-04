namespace DunIt.Core.ViewModels;

using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.Schedules;

public class AdminViewModel
{
    private readonly IChoreRepository _choreRepository;
    private readonly IChildRepository _childRepository;
    private Dictionary<string, IReadOnlyList<Chore>> _choresByChildId = new();

    public IReadOnlyList<Child> Children { get; private set; } = [];

    public IEnumerable<Chore> ChoresFor(Child child) =>
        _choresByChildId.TryGetValue(child.Id, out var chores) ? chores : [];

    public AdminViewModel(IChoreRepository choreRepository, IChildRepository childRepository)
    {
        _choreRepository = choreRepository;
        _childRepository = childRepository;
    }

    public async Task Initialize()
    {
        Children = await _childRepository.GetChildren();
        await RefreshChores();
    }

    public async Task AddChild(string name)
    {
        await _childRepository.AddChild(new Child(Guid.NewGuid().ToString(), name));
        await Refresh();
    }

    public async Task DeleteChild(Child child)
    {
        var chores = await _choreRepository.GetChoresForChild(child.Id);
        foreach (var chore in chores)
            await _choreRepository.DeleteChore(chore.Id);
        await _childRepository.DeleteChild(child.Id);
        await Refresh();
    }

    public async Task AddChore(Child child, string title, ChoreSchedule schedule)
    {
        await _choreRepository.AddChore(new Chore(Guid.NewGuid().ToString(), title, child.Id, schedule));
        await RefreshChoresFor(child);
    }

    public async Task DeleteChore(Chore chore)
    {
        await _choreRepository.DeleteChore(chore.Id);
        await RefreshChoresFor(chore.AssignedTo);
    }

    private async Task Refresh()
    {
        Children = await _childRepository.GetChildren();
        await RefreshChores();
    }

    private async Task RefreshChores()
    {
        var dict = new Dictionary<string, IReadOnlyList<Chore>>();
        foreach (var child in Children)
            dict[child.Id] = await _choreRepository.GetChoresForChild(child.Id);
        _choresByChildId = dict;
    }

    private async Task RefreshChoresFor(Child child) => await RefreshChoresFor(child.Id);

    private async Task RefreshChoresFor(string childId)
    {
        _choresByChildId[childId] = await _choreRepository.GetChoresForChild(childId);
    }
}

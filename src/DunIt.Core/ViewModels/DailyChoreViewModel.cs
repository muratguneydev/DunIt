namespace DunIt.Core.ViewModels;

using DunIt.Core.Models;
using DunIt.Core.Repositories;

public class DailyChoreViewModel
{
    private readonly IChoreRepository _choreRepository;
    private readonly IChildRepository _childRepository;

    public IReadOnlyList<Child> Children { get; private set; } = [];
    public Child? SelectedChild { get; private set; }
    public IReadOnlyList<Chore> UncompletedChores { get; private set; } = [];
    public IReadOnlyList<CompletedChore> CompletedChores { get; private set; } = [];
    public int TotalCount => UncompletedChores.Count + CompletedChores.Count;
    public int CompletedCount => CompletedChores.Count;

    public DailyChoreViewModel(IChoreRepository choreRepository, IChildRepository childRepository)
    {
        _choreRepository = choreRepository;
        _childRepository = childRepository;
    }

    public async Task Initialize()
    {
        Children = await _childRepository.GetChildren();
        if (Children.Count > 0)
            await SelectChild(Children[0]);
    }

    public async Task SelectChild(Child child)
    {
        SelectedChild = child;
        await RefreshChores();
    }

    public async Task Complete(Chore chore)
    {
        await _choreRepository.CompleteChore(chore.Id, SelectedChild!.Id, DateTimeOffset.Now);
        await RefreshChores();
    }

    public async Task Undo(CompletedChore completed)
    {
        await _choreRepository.UndoChore(completed.Completion.Id);
        await RefreshChores();
    }

    private async Task RefreshChores()
    {
        var today = DateTimeOffset.Now;
        var allChores = await _choreRepository.GetChoresForChild(SelectedChild!.Id);
        var todaysChores = allChores.Where(c => c.IsScheduledFor(today)).ToList();
        var completions = await _choreRepository.GetCompletionsFor(SelectedChild.Id, today);

        var completedChoreIds = completions.ToDictionary(c => c.ChoreId);
        CompletedChores = todaysChores
            .Where(c => completedChoreIds.ContainsKey(c.Id))
            .Select(c => new CompletedChore(c, completedChoreIds[c.Id]))
            .ToList();
        UncompletedChores = todaysChores
            .Where(c => !completedChoreIds.ContainsKey(c.Id))
            .ToList();
    }
}

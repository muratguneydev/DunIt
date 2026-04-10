namespace DunIt.Core.ViewModels;

using DunIt.Core.Models;
using DunIt.Core.Repositories;

public class DailyChoreViewModel : IAsyncDisposable
{
    private readonly IChoreRepository _choreRepository;
    private readonly IChildRepository _childRepository;

    private IReadOnlyList<Chore> _cachedChores = [];
    private IReadOnlyList<ChoreCompletion> _cachedCompletions = [];
    private ISubscription _childrenSub = NullSubscription.Instance;
    private ISubscription _choresSub = NullSubscription.Instance;
    private ISubscription _completionsSub = NullSubscription.Instance;

    public event Action StateChanged = delegate { };

    public bool IsChildView { get; private set; }
    public IReadOnlyList<Child> Children { get; private set; } = [];
    public Child SelectedChild { get; private set; } = Child.Empty;
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
        IsChildView = false;
        await DisposeAsync();

        Children = await _childRepository.GetChildren();
        if (Children.Count > 0)
            await SelectChild(Children[0]);

        _childrenSub = await _childRepository.Subscribe(updatedChildren =>
        {
            Children = updatedChildren;
            StateChanged();
        }) ?? NullSubscription.Instance;
    }

    public async Task InitializeAsChild(FirebaseUid childUid)
    {
        IsChildView = true;
        await DisposeAsync();

        Children = await _childRepository.GetChildren();
        var myChild = Children.FirstOrDefault(c => c.FirebaseUid == childUid);
        if (myChild != null)
            await SelectChild(myChild);
    }

    public async Task SelectChild(Child child)
    {
        SelectedChild = child;
        await RefreshChores();
        await SubscribeToSelectedChild();
    }

    public async Task Complete(Chore chore)
    {
        await _choreRepository.CompleteChore(chore.Id, SelectedChild.Id, DateTimeOffset.Now);
        await RefreshChores();
    }

    public async Task Undo(CompletedChore completed)
    {
        await _choreRepository.UndoChore(completed.Completion.Id);
        await RefreshChores();
    }

    public async ValueTask DisposeAsync()
    {
        await _childrenSub.DisposeAsync();
        await _choresSub.DisposeAsync();
        await _completionsSub.DisposeAsync();
        _childrenSub = NullSubscription.Instance;
        _choresSub = NullSubscription.Instance;
        _completionsSub = NullSubscription.Instance;
    }

    private async Task SubscribeToSelectedChild()
    {
        await _choresSub.DisposeAsync();
        await _completionsSub.DisposeAsync();

        var childId = SelectedChild.Id;
        var today = DateTimeOffset.Now;

        _choresSub = await _choreRepository.SubscribeToChores(childId, updatedChores =>
        {
            _cachedChores = updatedChores;
            UpdateChoreView();
            StateChanged();
        }) ?? NullSubscription.Instance;

        _completionsSub = await _choreRepository.SubscribeToCompletions(childId, today, updatedCompletions =>
        {
            _cachedCompletions = updatedCompletions;
            UpdateChoreView();
            StateChanged();
        }) ?? NullSubscription.Instance;
    }

    private async Task RefreshChores()
    {
        var today = DateTimeOffset.Now;
        _cachedChores = await _choreRepository.GetChoresForChild(SelectedChild.Id);
        _cachedCompletions = await _choreRepository.GetCompletionsFor(SelectedChild.Id, today);
        UpdateChoreView();
    }

    private void UpdateChoreView()
    {
        var today = DateTimeOffset.Now;
        var todaysChores = _cachedChores.Where(c => c.IsScheduledFor(today)).ToList();
        var completedChoreIds = _cachedCompletions.ToDictionary(c => c.ChoreId);
        CompletedChores = todaysChores
            .Where(c => completedChoreIds.ContainsKey(c.Id))
            .Select(c => new CompletedChore(c, completedChoreIds[c.Id]))
            .ToList();
        UncompletedChores = todaysChores.Where(c => !completedChoreIds.ContainsKey(c.Id)).ToList();
    }
}

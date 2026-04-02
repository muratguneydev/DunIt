namespace DunIt.Core.ViewModels;

using DunIt.Core.Models;

public class DailyChoreViewModel
{
    public IReadOnlyList<Chore> UncompletedChores { get; }
    public IReadOnlyList<CompletedChore> CompletedChores { get; }
    public int TotalCount => UncompletedChores.Count + CompletedChores.Count;
    public int CompletedCount => CompletedChores.Count;

    public DailyChoreViewModel(IReadOnlyList<Chore> todaysChores, IReadOnlyList<ChoreCompletion> completions)
    {
        var completedChoreIds = completions.ToDictionary(c => c.ChoreId);

        CompletedChores = todaysChores
            .Where(chore => completedChoreIds.ContainsKey(chore.Id))
            .Select(chore => new CompletedChore(chore, completedChoreIds[chore.Id]))
            .ToList();

        UncompletedChores = todaysChores
            .Where(chore => !completedChoreIds.ContainsKey(chore.Id))
            .ToList();
    }
}

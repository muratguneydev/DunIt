namespace DunIt.Core.ViewModels;

using DunIt.Core.Models;
using DunIt.Core.Repositories;

public class WeeklyReportViewModel(IChoreRepository choreRepository, IChildRepository childRepository)
{
    public bool IsChildView { get; private set; }
    public IReadOnlyList<Child> Children { get; private set; } = [];
    public Child SelectedChild { get; private set; } = Child.Empty;
    public IReadOnlyList<DayReport> Days { get; private set; } = [];
    public int TotalCompleted => Days.Sum(d => d.CompletedCount);
    public int TotalDue => Days.Sum(d => d.TotalCount);

    public async Task Initialize()
    {
        IsChildView = false;
        Children = await childRepository.GetChildren();
        if (Children.Count > 0)
            await SelectChild(Children[0]);
    }

    public async Task InitializeAsChild(FirebaseUid childUid)
    {
        IsChildView = true;
        Children = await childRepository.GetChildren();
        var myChild = Children.FirstOrDefault(c => c.FirebaseUid == childUid);
        if (myChild != null)
            await SelectChild(myChild);
    }

    public async Task SelectChild(Child child)
    {
        SelectedChild = child;
        Days = await LoadReport(child);
    }

    private async Task<IReadOnlyList<DayReport>> LoadReport(Child child)
    {
        var today = DateTimeOffset.Now;
        var chores = await choreRepository.GetChoresForChild(child.Id);
        var days = new List<DayReport>();
        for (var i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var completions = await choreRepository.GetCompletionsFor(child.Id, date);
            var dueChores = chores.Where(c => c.IsScheduledFor(date)).ToList();
            var completedCount = dueChores.Count(c => completions.Any(comp => comp.ChoreId == c.Id));
            days.Add(new DayReport(date, dueChores.Count, completedCount));
        }
        return days;
    }
}

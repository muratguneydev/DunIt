namespace DunIt.Core.Models;

using DunIt.Core.Schedules;

public record Chore(ChoreId Id, string Title, ChildId AssignedTo, ChoreSchedule Schedule)
{
    public bool IsScheduledFor(DateTimeOffset dateTime) => Schedule.IsScheduledFor(dateTime);
}

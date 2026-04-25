namespace DunIt.Core.Models;

using DunIt.Core.Schedules;

public record Chore(ChoreId Id, string Title, ChildId AssignedTo, ChoreSchedule Schedule)
{
    public TimeOnly DueBy { get; init; } = new TimeOnly(23, 59);

    public bool IsScheduledFor(DateTimeOffset dateTime) => Schedule.IsScheduledFor(dateTime);
}

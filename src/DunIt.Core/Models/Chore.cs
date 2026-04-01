namespace DunIt.Core.Models;

using DunIt.Core.Schedules;

public record Chore(string Id, string Title, string AssignedTo, ChoreSchedule Schedule)
{
    public bool IsScheduledFor(DateTimeOffset dateTime) => Schedule.IsScheduledFor(dateTime);
}

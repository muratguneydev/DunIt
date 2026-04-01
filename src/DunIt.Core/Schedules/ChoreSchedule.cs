namespace DunIt.Core.Schedules;

public abstract record ChoreSchedule
{
    public abstract bool IsScheduledFor(DateTimeOffset dateTime);
}

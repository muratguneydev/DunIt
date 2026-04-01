namespace DunIt.Core.Schedules;

public record DailySchedule : ChoreSchedule
{
    public override bool IsScheduledFor(DateTimeOffset dateTime) => true;
}

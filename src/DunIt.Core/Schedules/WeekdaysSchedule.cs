namespace DunIt.Core.Schedules;

public record WeekdaysSchedule : ChoreSchedule
{
    public override bool IsScheduledFor(DateTimeOffset dateTime) =>
        dateTime.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
}

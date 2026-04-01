namespace DunIt.Core.Schedules;

public record WeekendsSchedule : ChoreSchedule
{
    public override bool IsScheduledFor(DateTimeOffset dateTime) =>
        dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
}

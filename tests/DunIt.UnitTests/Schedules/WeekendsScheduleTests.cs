namespace DunIt.UnitTests.Schedules;

using DunIt.Core.Schedules;
using NUnit.Framework;
using Shouldly;

public class WeekendsScheduleTests
{
    [TestCase(DayOfWeek.Saturday)]
    [TestCase(DayOfWeek.Sunday)]
    public void ShouldBeScheduled_WhenWeekend(DayOfWeek day)
    {
        var dateTime = NextDateTimeOffsetFor(day);
        new WeekendsSchedule().IsScheduledFor(dateTime).ShouldBeTrue();
    }

    [TestCase(DayOfWeek.Monday)]
    [TestCase(DayOfWeek.Tuesday)]
    [TestCase(DayOfWeek.Wednesday)]
    [TestCase(DayOfWeek.Thursday)]
    [TestCase(DayOfWeek.Friday)]
    public void ShouldNotBeScheduled_WhenWeekday(DayOfWeek day)
    {
        var dateTime = NextDateTimeOffsetFor(day);
        new WeekendsSchedule().IsScheduledFor(dateTime).ShouldBeFalse();
    }

    private static DateTimeOffset NextDateTimeOffsetFor(DayOfWeek day)
    {
        var date = DateTimeOffset.UtcNow;
        var daysUntil = ((int)day - (int)date.DayOfWeek + 7) % 7;
        return date.AddDays(daysUntil == 0 ? 7 : daysUntil);
    }
}

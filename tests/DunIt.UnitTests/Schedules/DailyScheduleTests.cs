namespace DunIt.UnitTests.Schedules;

using AutoFixture.NUnit4;
using DunIt.Core.Schedules;
using NUnit.Framework;
using Shouldly;

public class DailyScheduleTests
{
    [Test, AutoData]
    public void ShouldBeScheduled_WhenAnyDay(DateTimeOffset dateTime)
    {
        new DailySchedule().IsScheduledFor(dateTime).ShouldBeTrue();
    }
}

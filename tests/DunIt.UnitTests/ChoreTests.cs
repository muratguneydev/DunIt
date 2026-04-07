namespace DunIt.UnitTests;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Schedules;
using NUnit.Framework;
using Shouldly;

public class ChoreTests
{
    [Test, AutoData]
    public void ShouldBeScheduled_WhenScheduleSaysTrue(ChoreId id, string title, ChildId assignedTo, DateTimeOffset dateTime)
    {
        // Arrange
        var chore = new Chore(id, title, assignedTo, new DailySchedule());

        // Act / Assert
        chore.IsScheduledFor(dateTime).ShouldBeTrue();
    }

    [Test, AutoData]
    public void ShouldNotBeScheduled_WhenScheduleSaysFalse(ChoreId id, string title, ChildId assignedTo)
    {
        // Arrange
        var weekday = new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero); // Wednesday
        var chore = new Chore(id, title, assignedTo, new WeekendsSchedule());

        // Act / Assert
        chore.IsScheduledFor(weekday).ShouldBeFalse();
    }
}

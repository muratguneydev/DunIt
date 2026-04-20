namespace DunIt.UnitTests.Notifications;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Notifications;
using DunIt.Testing;
using Moq;
using NUnit.Framework;

public class ReminderSchedulerTests
{
    [Test, AutoMoqData]
    public async Task ShouldScheduleReminder_WhenEnabledAndTimeSet(
        [Frozen, Mock] Mock<ReminderTimer> timerSpy,
        [Frozen] Mock<IClock> clockStub,
        ReminderScheduler sut)
    {
        // Arrange
        var now = new DateTimeOffset(2026, 4, 20, 6, 0, 0, TimeSpan.Zero);
        clockStub.Setup(c => c.Now).Returns(now);
        var settings = new ReminderSettings(Enabled: true, Time: new TimeOnly(7, 0));

        // Act
        await sut.ScheduleDaily(settings);

        // Assert
        timerSpy.Verify(t => t.Schedule(TimeSpan.FromHours(1), It.IsAny<Func<Task>>()));
    }

    [Test, AutoMoqData]
    public async Task ShouldCancelReminder_WhenDisabled(
        [Frozen, Mock] Mock<ReminderTimer> timerSpy,
        ReminderScheduler sut)
    {
        // Arrange
        var settings = new ReminderSettings(Enabled: false, Time: new TimeOnly(7, 0));

        // Act
        await sut.ScheduleDaily(settings);

        // Assert
        timerSpy.Verify(t => t.Cancel());
        timerSpy.Verify(t => t.Schedule(It.IsAny<TimeSpan>(), It.IsAny<Func<Task>>()), Times.Never);
    }
}

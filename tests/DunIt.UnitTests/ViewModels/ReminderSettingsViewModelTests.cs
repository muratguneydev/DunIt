namespace DunIt.UnitTests.ViewModels;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Notifications;
using DunIt.Core.ViewModels;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class ReminderSettingsViewModelTests
{
    [Test, AutoMoqData]
    public async Task ShouldDisplayCurrentSettings_WhenLoaded(
        ReminderSettings settings,
        [Frozen, Mock] Mock<ReminderSettingsService> settingsStub,
        [Frozen, Mock] Mock<ReminderScheduler> schedulerDummy,
        ReminderSettingsViewModel sut)
    {
        // Arrange
        settingsStub.Setup(s => s.GetSettingsAsync()).ReturnsAsync(settings);

        // Act
        await sut.LoadAsync();

        // Assert
        sut.Enabled.ShouldBe(settings.Enabled);
        sut.Time.ShouldBe(settings.Time);
    }

    [Test, AutoMoqData]
    public async Task ShouldUpdateSettings_WhenSaved(
        TimeOnly time,
        [Frozen, Mock] Mock<ReminderSettingsService> settingsSpy,
        [Frozen, Mock] Mock<ReminderScheduler> schedulerSpy,
        ReminderSettingsViewModel sut)
    {
        // Arrange
        sut.Enabled = true;
        sut.Time = time;

        // Act
        await sut.SaveAsync();

        // Assert
        settingsSpy.Verify(s => s.SaveSettingsAsync(new ReminderSettings(true, time)));
        schedulerSpy.Verify(s => s.ScheduleDaily(new ReminderSettings(true, time)));
    }
}

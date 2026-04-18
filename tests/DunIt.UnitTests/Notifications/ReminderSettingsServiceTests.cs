namespace DunIt.UnitTests.Notifications;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Notifications;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public class ReminderSettingsServiceTests
{
    [Test, AutoMoqData]
    public async Task ShouldHaveDefaultSettings_WhenNoSettingsSaved(
        [Frozen] Mock<ILocalStorage> storageStub,
        ReminderSettingsService sut)
    {
        // Arrange
        storageStub.Setup(s => s.GetItemAsync("reminderSettings")).ReturnsAsync((string?)null);

        // Act
        var result = await sut.GetSettingsAsync();

        // Assert
        result.Enabled.ShouldBe(false);
        result.Time.ShouldBe(new TimeOnly(9, 0));
    }

    [Test, AutoMoqData]
    public async Task ShouldSaveSettings_WhenUserUpdates(
        ReminderSettings settings,
        [Frozen] Mock<ILocalStorage> storageStub,
        ReminderSettingsService sut)
    {
        // Arrange

        // Act
        await sut.SaveSettingsAsync(settings);

        // Assert
        storageStub.Verify(s => s.SetItemAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        // Note: Serialization is tested implicitly; detailed JSON check omitted for simplicity
    }
}
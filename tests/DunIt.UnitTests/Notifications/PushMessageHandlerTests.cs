namespace DunIt.UnitTests.Notifications;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Notifications;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using System.Text.Json;

public class PushMessageHandlerTests
{
    [Test, AutoMoqData]
    public async Task ShouldShowNotification_WhenPushReceived(
        [Frozen] Mock<IServiceWorkerInterop> interopSpy,
        PushMessageHandler sut)
    {
        // Arrange
        var payload = new PushPayload("Time to do chores!", "Don't forget your daily tasks.");
        var payloadJson = JsonSerializer.Serialize(payload);

        // Act
        await sut.HandlePush(payloadJson);

        // Assert
        interopSpy.Verify(i => i.ShowNotification(payload.Title, payload.Body));
    }

    [Test, AutoMoqData]
    public async Task ShouldOpenApp_WhenNotificationClicked(
        [Frozen] Mock<IServiceWorkerInterop> interopSpy,
        PushMessageHandler sut)
    {
        // Act
        await sut.HandleNotificationClick();

        // Assert
        interopSpy.Verify(i => i.OpenApp());
    }
}

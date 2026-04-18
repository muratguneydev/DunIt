namespace DunIt.UnitTests.Notifications;

using AutoFixture.NUnit4;
using DunIt.Core.Notifications;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class NotificationServiceTests
{
    [Test, AutoMoqData]
    public async Task ShouldRequestPermission_WhenInitializing(
        [Frozen] Mock<INotificationInterop> interopStub,
        NotificationService sut)
    {
        // Arrange
        interopStub.Setup(i => i.RequestPermission())
                   .ReturnsAsync(NotificationPermission.Granted);

        // Act
        var permission = await sut.RequestPermission();

        // Assert
        permission.ShouldBe(NotificationPermission.Granted);
        interopStub.Verify(i => i.RequestPermission());
    }

    [Test, AutoMoqData]
    public async Task ShouldHandlePermissionDenied_WhenUserDeclines(
        [Frozen] Mock<INotificationInterop> interopStub,
        NotificationService sut)
    {
        // Arrange
        interopStub.Setup(i => i.RequestPermission())
                   .ReturnsAsync(NotificationPermission.Denied);

        // Act
        var permission = await sut.RequestPermission();

        // Assert
        permission.ShouldBe(NotificationPermission.Denied);
        sut.IsPermissionGranted.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldTrackGrantedPermission_WhenUserAllows(
        [Frozen] Mock<INotificationInterop> interopStub,
        NotificationService sut)
    {
        // Arrange
        interopStub.Setup(i => i.RequestPermission())
                   .ReturnsAsync(NotificationPermission.Granted);

        // Act
        await sut.RequestPermission();

        // Assert
        sut.IsPermissionGranted.ShouldBeTrue();
    }
}

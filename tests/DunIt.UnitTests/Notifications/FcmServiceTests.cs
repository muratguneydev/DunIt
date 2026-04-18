namespace DunIt.UnitTests.Notifications;

using AutoFixture.NUnit4;
using DunIt.Core.Notifications;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class FcmServiceTests
{
    [Test, AutoMoqData]
    public async Task ShouldGetFcmToken_WhenPermissionGranted(
        string validToken,
        [Frozen] Mock<IFcmInterop> interopStub,
        FcmService sut)
    {
        // Arrange
        interopStub.Setup(i => i.GetToken())
                   .ReturnsAsync(validToken);

        // Act
        var token = await sut.GetToken();

        // Assert
        token.ShouldBe(validToken);
        interopStub.Verify(i => i.GetToken(), Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldHandleTokenError_WhenFcmFails(
        [Frozen] Mock<IFcmInterop> interopStub,
        FcmService sut)
    {
        // Arrange
        interopStub.Setup(i => i.GetToken())
                   .ThrowsAsync(new Exception("FCM error"));

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => sut.GetToken());
    }

    [Test, AutoMoqData]
    public async Task ShouldStoreTokenInLocalStorage_WhenTokenRetrieved(
        string validToken,
        [Frozen] Mock<IFcmInterop> interopStub,
        [Frozen] Mock<ILocalStorage> storageStub,
        FcmService sut)
    {
        // Arrange
        interopStub.Setup(i => i.GetToken())
                   .ReturnsAsync(validToken);

        // Act
        await sut.GetToken();

        // Assert
        storageStub.Verify(s => s.SetItemAsync("fcm_token", validToken), Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnStoredToken_WhenAvailable(
        string storedToken,
        [Frozen] Mock<ILocalStorage> storageStub,
        FcmService sut)
    {
        // Arrange
        storageStub.Setup(s => s.GetItemAsync("fcm_token"))
                   .ReturnsAsync(storedToken);

        // Act
        var token = await sut.GetStoredToken();

        // Assert
        token.ShouldBe(storedToken);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnNull_WhenNoStoredToken(
        [Frozen] Mock<ILocalStorage> storageStub,
        FcmService sut)
    {
        // Arrange
        storageStub.Setup(s => s.GetItemAsync("fcm_token"))
                   .ReturnsAsync((string?)null);

        // Act
        var token = await sut.GetStoredToken();

        // Assert
        token.ShouldBeNull();
    }
}

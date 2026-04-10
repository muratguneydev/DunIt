namespace DunIt.UnitTests.Auth;

using AutoFixture.NUnit4;
using DunIt.Core.Firebase;
using DunIt.Core.Models;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class UserContextTests
{
    // ── RestoreSession ───────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldBeAuthenticated_WhenSessionExists(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(true);
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        await sut.RestoreSession();

        // Assert
        sut.IsAuthenticated.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeAuthenticated_WhenNoSessionExists(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(false);

        // Act
        await sut.RestoreSession();

        // Assert
        sut.IsAuthenticated.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSessionRestored(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(true);
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);
        var fired = false;
        sut.Changed += () => fired = true;

        // Act
        await sut.RestoreSession();

        // Assert
        fired.ShouldBeTrue();
    }

    // ── SignIn ───────────────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldBeAuthenticated_WhenSignInSucceeds(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        await sut.SignIn();

        // Assert
        sut.IsAuthenticated.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnTrue_WhenSignInSucceeds(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        var result = await sut.SignIn();

        // Assert
        result.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeAuthenticated_WhenSignInFails(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn()).ThrowsAsync(new Exception("auth/popup-closed-by-user"));

        // Act
        await sut.SignIn();

        // Assert
        sut.IsAuthenticated.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnFalse_WhenSignInFails(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn()).ThrowsAsync(new Exception("auth/popup-closed-by-user"));

        // Act
        var result = await sut.SignIn();

        // Assert
        result.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSignInSucceeds(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);
        var fired = false;
        sut.Changed += () => fired = true;

        // Act
        await sut.SignIn();

        // Assert
        fired.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSignInFails(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn()).ThrowsAsync(new Exception("auth/popup-closed-by-user"));
        var fired = false;
        sut.Changed += () => fired = true;

        // Act
        await sut.SignIn();

        // Assert
        fired.ShouldBeTrue();
    }

    // ── SignOut ──────────────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldNotBeAuthenticated_WhenSignedOut(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Act
        await sut.SignOut();

        // Assert
        sut.IsAuthenticated.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSignedOut(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        var fired = false;
        sut.Changed += () => fired = true;

        // Act
        await sut.SignOut();

        // Assert
        fired.ShouldBeTrue();
    }

    // ── IsParent ─────────────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldBeParent_WhenUidExistsInParentsCollection(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(true);
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(true);

        // Act
        await sut.RestoreSession();

        // Assert
        sut.IsParent.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeParent_WhenUidNotInParentsCollection(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(true);
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        await sut.RestoreSession();

        // Assert
        sut.IsParent.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeParent_WhenNotAuthenticated(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(false);

        // Act
        await sut.RestoreSession();

        // Assert
        sut.IsParent.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldBeParent_WhenSignedInAndUidExistsInParentsCollection(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(true);

        // Act
        await sut.SignIn();

        // Assert
        sut.IsParent.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeParent_WhenSignedInAndUidNotInParentsCollection(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        await sut.SignIn();

        // Assert
        sut.IsParent.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeParent_WhenSignInFails(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        UserContext sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn()).ThrowsAsync(new Exception("auth/popup-closed-by-user"));

        // Act
        await sut.SignIn();

        // Assert
        sut.IsParent.ShouldBeFalse();
    }
}

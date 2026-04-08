namespace DunIt.UnitTests.Auth;

using AutoFixture.NUnit4;
using DunIt.Core.Auth;
using DunIt.Core.Firebase;
using DunIt.Core.Models;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class FirebaseAuthServiceTests
{
    // ── RestoreSession ───────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldBeAuthenticated_WhenSessionExists(
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
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
        FirebaseAuthService sut)
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
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.HasCurrentUser()).ReturnsAsync(true);
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);
        var fired = false;
        sut.AuthStateChanged += () => fired = true;

        // Act
        await sut.RestoreSession();

        // Assert
        fired.ShouldBeTrue();
    }

    // ── SignIn ───────────────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldBeAuthenticated_WhenSignInSucceeds(
        Credentials credentials,
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        await sut.SignIn(credentials);

        // Assert
        sut.IsAuthenticated.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnTrue_WhenSignInSucceeds(
        Credentials credentials,
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        var result = await sut.SignIn(credentials);

        // Assert
        result.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeAuthenticated_WhenSignInFails(
        Credentials credentials,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn(credentials)).ThrowsAsync(new Exception("auth/wrong-password"));

        // Act
        await sut.SignIn(credentials);

        // Assert
        sut.IsAuthenticated.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnFalse_WhenSignInFails(
        Credentials credentials,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn(credentials)).ThrowsAsync(new Exception("auth/wrong-password"));

        // Act
        var result = await sut.SignIn(credentials);

        // Assert
        result.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSignInSucceeds(
        Credentials credentials,
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);
        var fired = false;
        sut.AuthStateChanged += () => fired = true;

        // Act
        await sut.SignIn(credentials);

        // Assert
        fired.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSignInFails(
        Credentials credentials,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn(credentials)).ThrowsAsync(new Exception("auth/wrong-password"));
        var fired = false;
        sut.AuthStateChanged += () => fired = true;

        // Act
        await sut.SignIn(credentials);

        // Assert
        fired.ShouldBeTrue();
    }

    // ── SignOut ──────────────────────────────────────────────────────────────

    [Test, AutoMoqData]
    public async Task ShouldNotBeAuthenticated_WhenSignedOut(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Act
        await sut.SignOut();

        // Assert
        sut.IsAuthenticated.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireAuthStateChanged_WhenSignedOut(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        var fired = false;
        sut.AuthStateChanged += () => fired = true;

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
        FirebaseAuthService sut)
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
        FirebaseAuthService sut)
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
        FirebaseAuthService sut)
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
        Credentials credentials,
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(true);

        // Act
        await sut.SignIn(credentials);

        // Assert
        sut.IsParent.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeParent_WhenSignedInAndUidNotInParentsCollection(
        Credentials credentials,
        FirebaseUid uid,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetCurrentUserId()).ReturnsAsync(uid);
        firebaseInteropStub.Setup(f => f.IsParent(uid)).ReturnsAsync(false);

        // Act
        await sut.SignIn(credentials);

        // Assert
        sut.IsParent.ShouldBeFalse();
    }

    [Test, AutoMoqData]
    public async Task ShouldNotBeParent_WhenSignInFails(
        Credentials credentials,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseAuthService sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.SignIn(credentials)).ThrowsAsync(new Exception("auth/wrong-password"));

        // Act
        await sut.SignIn(credentials);

        // Assert
        sut.IsParent.ShouldBeFalse();
    }
}

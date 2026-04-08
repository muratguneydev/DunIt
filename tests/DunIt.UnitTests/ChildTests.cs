namespace DunIt.UnitTests;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using NUnit.Framework;
using Shouldly;

public class ChildTests
{
    [Test, AutoData]
    public void ShouldHaveDefaultAvatar_WhenAvatarNotProvided(ChildId id, string name)
    {
        // Act
        var child = new Child(id, name);

        // Assert
        child.ShouldBe(new Child(id, name, Avatar: "🧒"));
    }

    [Test, AutoData]
    public void ShouldHaveEmptyFirebaseUid_WhenFirebaseUidNotProvided(ChildId id, string name)
    {
        // Act
        var child = new Child(id, name);

        // Assert
        child.ShouldBe(new Child(id, name, FirebaseUid: default));
    }

    [Test, AutoData]
    public void ShouldIncludeFirebaseUid_WhenChildHasLinkedAccount(ChildId id, string name, FirebaseUid firebaseUid)
    {
        // Act
        var child = new Child(id, name, FirebaseUid: firebaseUid);

        // Assert
        child.ShouldBe(new Child(id, name, FirebaseUid: firebaseUid));
    }
}

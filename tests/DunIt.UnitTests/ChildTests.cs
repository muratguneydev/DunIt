namespace DunIt.UnitTests;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using NUnit.Framework;
using Shouldly;

public class ChildTests
{
    [Test, AutoData]
    public void ShouldHaveDefaultAvatar_WhenAvatarNotProvided(string id, string name)
    {
        // Act
        var child = new Child(id, name);

        // Assert
        child.Avatar.ShouldBe("🧒");
    }
}

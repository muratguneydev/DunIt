using AutoFixture.NUnit3;
using DunIt.Core.Models;
using NUnit.Framework;
using Shouldly;

namespace DunIt.UnitTests;

public class ChildTests
{
    [Test, AutoData]
    public void ShouldCreateChild_WhenNameProvided(string id, string name)
    {
        // Act
        var child = new Child(id, name);

        // Assert
        child.Id.ShouldBe(id);
        child.Name.ShouldBe(name);
        child.Avatar.ShouldBe("🧒");
    }
}

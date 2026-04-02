namespace DunIt.UnitTests.Repositories;

using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Testing;
using NUnit.Framework;
using Shouldly;

public class InMemoryChildRepositoryTests
{
    [Test, AutoMoqData]
    public async Task ShouldAddChild_WhenValidChildProvided(Child child, InMemoryChildRepository sut)
    {
        // Act
        var result = await sut.AddChild(child);

        // Assert
        result.ShouldBe(child);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnAllChildren_WhenChildrenExist(Child child1, Child child2, InMemoryChildRepository sut)
    {
        // Arrange
        await sut.AddChild(child1);
        await sut.AddChild(child2);

        // Act
        var result = await sut.GetChildren();

        // Assert
        result.ShouldBe([child1, child2]);
    }
}

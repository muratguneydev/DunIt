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
}

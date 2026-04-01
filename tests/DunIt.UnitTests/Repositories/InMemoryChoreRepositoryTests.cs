namespace DunIt.UnitTests.Repositories;

using AutoFixture.NUnit3;
using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.Schedules;
using DunIt.Testing;
using NUnit.Framework;
using Shouldly;

public class InMemoryChoreRepositoryTests
{
    [Test, DomainAutoData]
    public async Task ShouldAddChore_WhenValidChoreProvided(Chore chore,
        InMemoryChoreRepository sut)
    {
        // Act
        var result = await sut.AddChore(chore);
        // Assert
        result.ShouldBe(chore);
    }

    [Test, DomainAutoData]
    public async Task ShouldReturnChores_WhenChildHasAssignedChores(Chore childChore,
        Chore otherChore,
        InMemoryChoreRepository sut)
    {
        // Arrange
        await sut.AddChore(childChore);
        await sut.AddChore(otherChore);

        // Act
        var result = await sut.GetChoresForChild(childChore.AssignedTo);

        // Assert
        result.ShouldBe([childChore]);
    }
}

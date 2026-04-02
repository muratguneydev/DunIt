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

    [Test, DomainAutoData]
    public async Task ShouldRecordCompletion_WhenChoreCompleted(Chore chore, InMemoryChoreRepository sut)
    {
        // Arrange
        await sut.AddChore(chore);
        var completedAt = DateTimeOffset.UtcNow;

        // Act
        var completion = await sut.CompleteChore(chore.Id, chore.AssignedTo, completedAt);
        var completions = await sut.GetCompletionsFor(chore.AssignedTo, completedAt);

        // Assert
        completions.ShouldBe([completion]);
    }

    [Test, DomainAutoData]
    public async Task ShouldRemoveCompletion_WhenUndone(Chore chore, InMemoryChoreRepository sut)
    {
        // Arrange
        await sut.AddChore(chore);
        var completedAt = DateTimeOffset.UtcNow;
        var completion = await sut.CompleteChore(chore.Id, chore.AssignedTo, completedAt);

        // Act
        await sut.UndoChore(completion.Id);
        var completions = await sut.GetCompletionsFor(chore.AssignedTo, completedAt);

        // Assert
        completions.ShouldBeEmpty();
    }
}

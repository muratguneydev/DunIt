namespace DunIt.UnitTests.Repositories;

using AutoFixture.NUnit3;
using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.Schedules;
using NUnit.Framework;
using Shouldly;

public class InMemoryChoreRepositoryTests
{
    [Test, AutoData]
    public async Task ShouldAddChore_WhenValidChoreProvided(string id, string title, string assignedTo)
    {
        // Arrange
        var chore = new Chore(id, title, assignedTo, new DailySchedule());
        var repository = new InMemoryChoreRepository();

        // Act
        var result = await repository.AddChore(chore);

        // Assert
        result.ShouldBe(chore);
    }
}

namespace DunIt.UnitTests.ViewModels;

using DunIt.Core.Models;
using DunIt.Core.ViewModels;
using DunIt.Testing;
using NUnit.Framework;
using Shouldly;

public class DailyChoreViewModelTests
{
    [Test, DomainAutoData]
    public void ShouldReturnOnlyTodaysChores_WhenChoresHaveDifferentSchedules(
        Chore chore1, Chore chore2, ChoreCompletion completion)
    {
        // Act
        var viewModel = new DailyChoreViewModel([chore1, chore2], []);

        // Assert
        viewModel.UncompletedChores.ShouldBe([chore1, chore2]);
        viewModel.CompletedChores.ShouldBeEmpty();
    }

    [Test, DomainAutoData]
    public void ShouldShowCompleted_WhenChoreHasCompletionForToday(
        Chore chore1, Chore chore2, ChoreCompletion completion)
    {
        // Arrange
        var matchingCompletion = completion with { ChoreId = chore1.Id };

        // Act
        var viewModel = new DailyChoreViewModel([chore1, chore2], [matchingCompletion]);

        // Assert
        viewModel.CompletedChores.ShouldBe([new CompletedChore(chore1, matchingCompletion)]);
        viewModel.UncompletedChores.ShouldBe([chore2]);
    }

    [Test, DomainAutoData]
    public void ShouldCalculateProgress_WhenSomeChoresCompleted(
        Chore chore1, Chore chore2, Chore chore3, ChoreCompletion completion)
    {
        // Arrange
        var matchingCompletion = completion with { ChoreId = chore1.Id };

        // Act
        var viewModel = new DailyChoreViewModel([chore1, chore2, chore3], [matchingCompletion]);

        // Assert
        viewModel.TotalCount.ShouldBe(3);
        viewModel.CompletedCount.ShouldBe(1);
    }
}

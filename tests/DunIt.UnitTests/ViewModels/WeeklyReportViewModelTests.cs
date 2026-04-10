namespace DunIt.UnitTests.ViewModels;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.ViewModels;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class WeeklyReportViewModelTests
{
    [Test, AutoMoqData]
    public async Task ShouldLoadLast7Days_WhenInitialized(
        Child child,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        WeeklyReportViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, It.IsAny<DateTimeOffset>())).ReturnsAsync([]);

        // Act
        await sut.Initialize();

        // Assert
        sut.Days.Count.ShouldBe(7);
    }

    [Test, AutoMoqData]
    public async Task ShouldCountCompletedChores_WhenChoreCompletedToday(
        Child child, Chore chore, ChoreCompletion completion,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        WeeklyReportViewModel sut)
    {
        // Arrange
        var matchingCompletion = completion with { ChoreId = chore.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(DateTimeOffset.Now, 5))).ReturnsAsync([matchingCompletion]);

        // Act
        await sut.Initialize();

        // Assert
        var today = sut.Days.Last();
        today.CompletedCount.ShouldBe(1);
        today.TotalCount.ShouldBe(1);
    }

    [Test, AutoMoqData]
    public async Task ShouldSumCompletionsAcrossWeek(
        Child child, Chore chore1, Chore chore2,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        WeeklyReportViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore1, chore2]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, It.IsAny<DateTimeOffset>())).ReturnsAsync([]);

        // Act
        await sut.Initialize();

        // Assert
        sut.TotalDue.ShouldBe(14); // 2 daily chores × 7 days
        sut.TotalCompleted.ShouldBe(0);
    }

    [Test, AutoMoqData]
    public async Task ShouldAutoSelectChild_WhenInitializedAsChild(
        Child child1, Child child2,
        [Frozen] Mock<IChoreRepository> choreRepoDummy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        WeeklyReportViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child1, child2]);
        choreRepoDummy.Setup(r => r.GetChoresForChild(child1.Id)).ReturnsAsync([]);
        choreRepoDummy.Setup(r => r.GetCompletionsFor(child1.Id, It.IsAny<DateTimeOffset>())).ReturnsAsync([]);

        // Act
        await sut.InitializeAsChild(child1.FirebaseUid);

        // Assert
        sut.SelectedChild.ShouldBe(child1);
        sut.IsChildView.ShouldBeTrue();
    }

    [Test, AutoMoqData]
    public async Task ShouldSwitchReport_WhenChildSelected(
        Child child1, Child child2, Chore chore1, Chore chore2,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        WeeklyReportViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child1, child2]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child1.Id)).ReturnsAsync([chore1]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child2.Id)).ReturnsAsync([chore2]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(It.IsAny<ChildId>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        await sut.Initialize();

        // Act
        await sut.SelectChild(child2);

        // Assert
        sut.SelectedChild.ShouldBe(child2);
        sut.TotalDue.ShouldBe(7); // 1 daily chore × 7 days
    }

    private static DateTimeOffset WithinSeconds(DateTimeOffset reference, int seconds) =>
        It.Is<DateTimeOffset>(dt => Math.Abs((dt - reference).TotalSeconds) < seconds);
}

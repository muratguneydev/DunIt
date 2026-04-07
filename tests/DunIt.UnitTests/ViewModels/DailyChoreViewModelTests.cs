namespace DunIt.UnitTests.ViewModels;

using AutoFixture;
using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.ViewModels;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class DailyChoreViewModelTests
{
    [Test, AutoMoqData]
    public async Task ShouldLoadChildren_WhenInitialized(
        List<Child> children,
        [Frozen] Mock<IChoreRepository> choreRepoDummy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var approxNow = DateTimeOffset.Now;
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync(children);
        choreRepoDummy.Setup(r => r.GetChoresForChild(children[0].Id)).ReturnsAsync([]);
        choreRepoDummy.Setup(r => r.GetCompletionsFor(children[0].Id, WithinSeconds(approxNow, 5))).ReturnsAsync([]);

        // Act
        await sut.Initialize();

        // Assert
        sut.Children.ShouldBe(children);
        sut.SelectedChild.ShouldBe(children[0]);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnOnlyTodaysChores_WhenChoresLoaded(
        Child child, List<Chore> chores,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var approxNow = DateTimeOffset.Now;
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync(chores);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(approxNow, 5))).ReturnsAsync([]);

        // Act
        await sut.Initialize();

        // Assert
        sut.UncompletedChores.ShouldBe(chores);
        sut.CompletedChores.ShouldBeEmpty();
    }

    [Test, AutoMoqData]
    public async Task ShouldShowCompleted_WhenChoreHasCompletionForToday(
        Child child, Chore chore1, Chore chore2, ChoreCompletion completion,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var approxNow = DateTimeOffset.Now;
        var matchingCompletion = completion with { ChoreId = chore1.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore1, chore2]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(approxNow, 5))).ReturnsAsync([matchingCompletion]);

        // Act
        await sut.Initialize();

        // Assert
        sut.CompletedChores.ShouldBe([new CompletedChore(chore1, matchingCompletion)]);
        sut.UncompletedChores.ShouldBe([chore2]);
    }

    [Test, AutoMoqData]
    public async Task ShouldCalculateProgress_WhenSomeChoresCompleted(
        Child child, Chore chore1, Chore chore2, Chore chore3, ChoreCompletion completion,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var approxNow = DateTimeOffset.Now;
        var matchingCompletion = completion with { ChoreId = chore1.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore1, chore2, chore3]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(approxNow, 5))).ReturnsAsync([matchingCompletion]);

        // Act
        await sut.Initialize();

        // Assert
        sut.TotalCount.ShouldBe(3);
        sut.CompletedCount.ShouldBe(1);
    }

    [Test, AutoMoqData]
    public async Task ShouldSwitchToChildsChores_WhenChildSelected(
        Child child1, Child child2, Chore child1Chore, Chore child2Chore,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var approxNow = DateTimeOffset.Now;
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child1, child2]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child1.Id)).ReturnsAsync([child1Chore]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child2.Id)).ReturnsAsync([child2Chore]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(It.IsAny<string>(), WithinSeconds(approxNow, 5))).ReturnsAsync([]);
        await sut.Initialize();

        // Act
        await sut.SelectChild(child2);

        // Assert
        sut.SelectedChild.ShouldBe(child2);
        sut.UncompletedChores.ShouldBe([child2Chore]);
    }

    [Test, AutoMoqData]
    public async Task ShouldMarkChoreComplete_WhenCompleted(
        Child child, Chore chore, ChoreCompletion completion,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var matchingCompletion = completion with { ChoreId = chore.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(DateTimeOffset.Now, 5))).ReturnsAsync([]);
        choreRepoStub.Setup(r => r.CompleteChore(chore.Id, child.Id, WithinSeconds(DateTimeOffset.Now, 5))).ReturnsAsync(matchingCompletion);
        await sut.Initialize();

        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(DateTimeOffset.Now, 5))).ReturnsAsync([matchingCompletion]);

        // Act
        await sut.Complete(chore);

        // Assert
        sut.CompletedChores.ShouldBe([new CompletedChore(chore, matchingCompletion)]);
        sut.UncompletedChores.ShouldBeEmpty();
    }

    [Test, AutoMoqData]
    public async Task ShouldMarkChoreUncompleted_WhenUndone(
        Child child, Chore chore, ChoreCompletion completion,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var matchingCompletion = completion with { ChoreId = chore.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(DateTimeOffset.Now, 5))).ReturnsAsync([matchingCompletion]);
        await sut.Initialize();

        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, WithinSeconds(DateTimeOffset.Now, 5))).ReturnsAsync([]);

        // Act
        await sut.Undo(new CompletedChore(chore, matchingCompletion));

        // Assert
        sut.UncompletedChores.ShouldBe([chore]);
        sut.CompletedChores.ShouldBeEmpty();
    }

    [Test, AutoMoqData]
    public async Task ShouldFireStateChanged_WhenChildrenSubscriptionUpdates(
        List<Child> updatedChildren,
        [Frozen] Mock<IChoreRepository> choreRepoDummy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([]);
        Action<IReadOnlyList<Child>>? capturedCallback = null;
        childRepoStub
            .Setup(r => r.Subscribe(It.IsAny<Action<IReadOnlyList<Child>>>()))
            .Callback<Action<IReadOnlyList<Child>>>(cb => capturedCallback = cb)
            .ReturnsAsync(Mock.Of<ISubscription>());
        await sut.Initialize();
        var stateChangedFired = false;
        sut.StateChanged += () => stateChangedFired = true;

        // Act
        capturedCallback!(updatedChildren);

        // Assert
        stateChangedFired.ShouldBeTrue();
        sut.Children.ShouldBe(updatedChildren);
    }

    [Test, AutoMoqData]
    public async Task ShouldFireStateChanged_WhenChoreSubscriptionFires(
        Child child, List<Chore> updatedChores,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        Action<IReadOnlyList<Chore>>? capturedCallback = null;
        choreRepoStub
            .Setup(r => r.SubscribeToChores(child.Id, It.IsAny<Action<IReadOnlyList<Chore>>>()))
            .Callback<string, Action<IReadOnlyList<Chore>>>((_, cb) => capturedCallback = cb)
            .ReturnsAsync(Mock.Of<ISubscription>());
        await sut.Initialize();
        var stateChangedFired = false;
        sut.StateChanged += () => stateChangedFired = true;

        // Act
        capturedCallback!(updatedChores);

        // Assert
        stateChangedFired.ShouldBeTrue();
        sut.UncompletedChores.ShouldBe(updatedChores);
    }

    [Test, AutoMoqData]
    public async Task ShouldFireStateChanged_WhenCompletionSubscriptionFires(
        Child child, Chore chore, ChoreCompletion completion,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        DailyChoreViewModel sut)
    {
        // Arrange
        var matchingCompletion = completion with { ChoreId = chore.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([chore]);
        choreRepoStub.Setup(r => r.GetCompletionsFor(child.Id, It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        Action<IReadOnlyList<ChoreCompletion>>? capturedCallback = null;
        choreRepoStub
            .Setup(r => r.SubscribeToCompletions(child.Id, It.IsAny<DateTimeOffset>(), It.IsAny<Action<IReadOnlyList<ChoreCompletion>>>()))
            .Callback<string, DateTimeOffset, Action<IReadOnlyList<ChoreCompletion>>>((_, __, cb) => capturedCallback = cb)
            .ReturnsAsync(Mock.Of<ISubscription>());
        await sut.Initialize();
        var stateChangedFired = false;
        sut.StateChanged += () => stateChangedFired = true;

        // Act
        capturedCallback!([matchingCompletion]);

        // Assert
        stateChangedFired.ShouldBeTrue();
        sut.CompletedChores.ShouldHaveSingleItem();
        sut.CompletedChores[0].Chore.ShouldBe(chore);
    }

    private static DateTimeOffset WithinSeconds(DateTimeOffset reference, int seconds) =>
        It.Is<DateTimeOffset>(dt => Math.Abs((dt - reference).TotalSeconds) < seconds);
}

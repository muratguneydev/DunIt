namespace DunIt.UnitTests.ViewModels;

using AutoFixture.NUnit4;
using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.Schedules;
using DunIt.Core.ViewModels;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class AdminViewModelTests
{
    [Test, AutoMoqData]
    public async Task ShouldLoadChildren_WhenInitialized(
        List<Child> children,
        [Frozen] Mock<IChoreRepository> choreRepoDummy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync(children);
        choreRepoDummy.Setup(r => r.GetChoresForChild(It.IsAny<string>())).ReturnsAsync([]);

        // Act
        await sut.Initialize();

        // Assert
        sut.Children.ShouldBe(children);
    }

    [Test, AutoMoqData]
    public async Task ShouldLoadChoresForChild_WhenInitialized(
        Child child,
        List<Chore> chores,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync(chores);

        // Act
        await sut.Initialize();

        // Assert
        sut.ChoresFor(child).ShouldBe(chores);
    }

    [Test, AutoMoqData]
    public async Task ShouldAddChild_WhenChildAdded(
        Child addedChild,
        [Frozen] Mock<IChoreRepository> choreRepoDummy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange — empty children means GetChoresForChild is never called during Initialize
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([]);
        await sut.Initialize();

        childRepoStub.Setup(r => r.AddChild(It.IsAny<Child>())).ReturnsAsync(addedChild);
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([addedChild]);
        choreRepoDummy.Setup(r => r.GetChoresForChild(addedChild.Id)).ReturnsAsync([]);

        // Act
        await sut.AddChild(addedChild.Name);

        // Assert
        sut.Children.ShouldContain(addedChild);
    }

    [Test, AutoMoqData]
    public async Task ShouldDeleteChild_WhenChildDeleted(
        Child child,
        [Frozen] Mock<IChoreRepository> choreRepoDummy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoDummy.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([]);
        await sut.Initialize();

        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([]);

        // Act
        await sut.DeleteChild(child);

        // Assert
        sut.Children.ShouldBeEmpty();
    }

    [Test, AutoMoqData]
    public async Task ShouldDeleteChildsChores_WhenChildDeleted(
        Child child,
        List<Chore> chores,
        [Frozen] Mock<IChoreRepository> choreRepoSpy,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoSpy.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync(chores);
        await sut.Initialize();

        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([]);

        // Act
        await sut.DeleteChild(child);

        // Assert
        foreach (var chore in chores)
            choreRepoSpy.Verify(r => r.DeleteChore(chore.Id), Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldAddChore_WhenChoreAdded(
        Child child,
        Chore addedChore,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([]);
        await sut.Initialize();

        choreRepoStub.Setup(r => r.AddChore(It.IsAny<Chore>())).ReturnsAsync(addedChore);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([addedChore]);

        // Act
        await sut.AddChore(child, addedChore.Title, new DailySchedule());

        // Assert
        sut.ChoresFor(child).ShouldContain(addedChore);
    }

    [Test, AutoMoqData]
    public async Task ShouldDeleteChore_WhenChoreDeleted(
        Child child,
        Chore chore,
        [Frozen] Mock<IChoreRepository> choreRepoStub,
        [Frozen] Mock<IChildRepository> childRepoStub,
        AdminViewModel sut)
    {
        // Arrange
        var assignedChore = chore with { AssignedTo = child.Id };
        childRepoStub.Setup(r => r.GetChildren()).ReturnsAsync([child]);
        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([assignedChore]);
        await sut.Initialize();

        choreRepoStub.Setup(r => r.GetChoresForChild(child.Id)).ReturnsAsync([]);

        // Act
        await sut.DeleteChore(assignedChore);

        // Assert
        sut.ChoresFor(child).ShouldBeEmpty();
    }
}
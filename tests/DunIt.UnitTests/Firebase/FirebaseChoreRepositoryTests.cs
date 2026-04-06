namespace DunIt.UnitTests.Firebase;

using AutoFixture.NUnit4;
using DunIt.Core.Firebase;
using DunIt.Core.Models;
using DunIt.Core.Schedules;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class FirebaseChoreRepositoryTests
{
    [Test, AutoMoqData]
    public async Task ShouldAddChore_WhenChoreProvided(
        string choreId, string title, string assignedTo,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var chore = new Chore(choreId, title, assignedTo, new DailySchedule());
        firebaseInteropSpy.Setup(f => f.AddChore(It.IsAny<ChoreDto>()))
            .ReturnsAsync(new ChoreDto(choreId, title, assignedTo, "daily"));

        // Act
        var result = await sut.AddChore(chore);

        // Assert
        firebaseInteropSpy.Verify(f => f.AddChore(
            It.Is<ChoreDto>(d => d.Id == choreId && d.Title == title && d.AssignedTo == assignedTo && d.ScheduleType == "daily")),
            Times.Once);
        result.Id.ShouldBe(choreId);
        result.Title.ShouldBe(title);
    }

    [Test, AutoMoqData]
    public async Task ShouldMapWeekdaysSchedule_WhenChoreHasWeekdaysSchedule(
        string choreId, string title, string assignedTo,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var chore = new Chore(choreId, title, assignedTo, new WeekdaysSchedule());
        firebaseInteropSpy.Setup(f => f.AddChore(It.IsAny<ChoreDto>()))
            .ReturnsAsync(new ChoreDto(choreId, title, assignedTo, "weekdays"));

        // Act
        await sut.AddChore(chore);

        // Assert
        firebaseInteropSpy.Verify(f => f.AddChore(
            It.Is<ChoreDto>(d => d.ScheduleType == "weekdays")),
            Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldDeleteChore_WhenChoreDeleted(
        string choreId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Act
        await sut.DeleteChore(choreId);

        // Assert
        firebaseInteropSpy.Verify(f => f.DeleteChore(choreId), Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnChores_WhenChildHasChores(
        string childId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseChoreRepository sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetChoresForChild(childId)).ReturnsAsync(
        [
            new ChoreDto("c1", "Make bed", childId, "daily"),
            new ChoreDto("c2", "Brush teeth", childId, "weekdays")
        ]);

        // Act
        var result = await sut.GetChoresForChild(childId);

        // Assert
        result.Count.ShouldBe(2);
        result[0].Id.ShouldBe("c1");
        result[0].Schedule.ShouldBeOfType<DailySchedule>();
        result[1].Id.ShouldBe("c2");
        result[1].Schedule.ShouldBeOfType<WeekdaysSchedule>();
    }

    [Test, AutoMoqData]
    public async Task ShouldCompleteChore_WhenChoreCompleted(
        string choreId, string childId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var completedAt = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        firebaseInteropSpy.Setup(f => f.CompleteChore(It.IsAny<ChoreCompletionDto>()))
            .ReturnsAsync((ChoreCompletionDto dto) => dto);

        // Act
        var result = await sut.CompleteChore(choreId, childId, completedAt);

        // Assert
        firebaseInteropSpy.Verify(f => f.CompleteChore(
            It.Is<ChoreCompletionDto>(d => d.ChoreId == choreId && d.ChildId == childId)),
            Times.Once);
        result.ChoreId.ShouldBe(choreId);
        result.ChildId.ShouldBe(childId);
        result.CompletedAt.ShouldBe(completedAt);
    }

    [Test, AutoMoqData]
    public async Task ShouldUndoChore_WhenCompletionUndone(
        string completionId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Act
        await sut.UndoChore(completionId);

        // Assert
        firebaseInteropSpy.Verify(f => f.UndoChore(completionId), Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnCompletions_WhenCompletionsExistForDate(
        string childId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var date = new DateTimeOffset(2026, 4, 5, 0, 0, 0, TimeSpan.Zero);
        firebaseInteropStub.Setup(f => f.GetCompletionsFor(childId, "2026-04-05")).ReturnsAsync(
        [
            new ChoreCompletionDto("comp-1", "chore-1", childId, "2026-04-05T10:00:00Z")
        ]);

        // Act
        var result = await sut.GetCompletionsFor(childId, date);

        // Assert
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe("comp-1");
        result[0].ChildId.ShouldBe(childId);
    }
}

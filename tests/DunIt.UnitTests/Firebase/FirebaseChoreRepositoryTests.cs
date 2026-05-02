namespace DunIt.UnitTests.Firebase;

using AutoFixture.NUnit4;
using DunIt.Core.Firebase;
using DunIt.Core.Models;
using DunIt.Core.Schedules;
using DunIt.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

public class FirebaseChoreRepositoryTests
{
    [Test, AutoMoqData]
    public async Task ShouldAddChore_WhenChoreProvided(
        ChoreId choreId, string title, ChildId assignedTo,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var chore = new Chore(choreId, title, assignedTo, new DailySchedule());
        firebaseInteropSpy.Setup(f => f.AddChore(It.IsAny<ChoreDto>()))
            .ReturnsAsync(new ChoreDto(choreId, title, assignedTo, "daily", "23:59"));

        // Act
        var result = await sut.AddChore(chore);

        // Assert
        firebaseInteropSpy.Verify(f => f.AddChore(
            It.Is<ChoreDto>(d => d.Id == choreId.Value && d.Title == title && d.AssignedTo == assignedTo.Value && d.ScheduleType == "daily")),
            Times.Once);
        result.Id.ShouldBe(choreId);
        result.Title.ShouldBe(title);
    }

    [Test, AutoMoqData]
    public async Task ShouldMapWeekdaysSchedule_WhenChoreHasWeekdaysSchedule(
        ChoreId choreId, string title, ChildId assignedTo,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var chore = new Chore(choreId, title, assignedTo, new WeekdaysSchedule());
        firebaseInteropSpy.Setup(f => f.AddChore(It.IsAny<ChoreDto>()))
            .ReturnsAsync(new ChoreDto(choreId, title, assignedTo, "weekdays", "23:59"));

        // Act
        await sut.AddChore(chore);

        // Assert
        firebaseInteropSpy.Verify(f => f.AddChore(
            It.Is<ChoreDto>(d => d.ScheduleType == "weekdays")),
            Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldDeleteChore_WhenChoreDeleted(
        ChoreId choreId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Act
        await sut.DeleteChore(choreId);

        // Assert
        firebaseInteropSpy.Verify(f => f.DeleteChore(choreId));
    }

    [Test, AutoMoqData]
    public async Task ShouldLogError_WhenChoreDtoHasInvalidDueBy(
        ChildId childId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        [Frozen] Mock<ILogger<FirebaseChoreRepository>> loggerMock,
        FirebaseChoreRepository sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetChoresForChild(childId)).ReturnsAsync(
        [
            new ChoreDto("c1", "Make bed", childId, "daily", "invalid")
        ]);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => sut.GetChoresForChild(childId));
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to convert ChoreDto to Chore")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldCompleteChore_WhenChoreCompleted(
        ChoreId choreId, ChildId childId,
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
            It.Is<ChoreCompletionDto>(d => d.ChoreId == choreId.Value && d.ChildId == childId.Value)),
            Times.Once);
        result.ChoreId.ShouldBe(choreId);
        result.ChildId.ShouldBe(childId);
        result.CompletedAt.ShouldBe(completedAt);
    }

    [Test, AutoMoqData]
    public async Task ShouldUndoChore_WhenCompletionUndone(
        ChoreCompletionId completionId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Act
        await sut.UndoChore(completionId);

        // Assert
        firebaseInteropSpy.Verify(f => f.UndoChore(completionId));
    }

    [Test, AutoMoqData]
    public async Task ShouldSerializeDueBy_WhenAddingChore(
        ChoreId choreId, string title, ChildId assignedTo,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChoreRepository sut)
    {
        // Arrange
        var dueBy = new TimeOnly(20, 0);
        var chore = new Chore(choreId, title, assignedTo, new DailySchedule()) { DueBy = dueBy };
        firebaseInteropSpy.Setup(f => f.AddChore(It.IsAny<ChoreDto>()))
            .ReturnsAsync(new ChoreDto(choreId, title, assignedTo, "daily", "20:00"));

        // Act
        await sut.AddChore(chore);

        // Assert
        firebaseInteropSpy.Verify(f => f.AddChore(
            It.Is<ChoreDto>(d => d.DueBy == "20:00")),
            Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldDeserializeDueBy_WhenGettingChores(
        ChildId childId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseChoreRepository sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetChoresForChild(childId)).ReturnsAsync(
        [
            new ChoreDto("c1", "Make bed", childId, "daily", "08:30")
        ]);

        // Act
        var result = await sut.GetChoresForChild(childId);

        // Assert
        result[0].DueBy.ShouldBe(new TimeOnly(8, 30));
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnCompletions_WhenCompletionsExistForDate(
        ChildId childId,
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
        result[0].Id.Value.ShouldBe("comp-1");
        result[0].ChildId.ShouldBe(childId);
    }
}

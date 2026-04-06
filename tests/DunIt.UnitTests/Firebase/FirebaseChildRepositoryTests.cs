namespace DunIt.UnitTests.Firebase;

using AutoFixture.NUnit4;
using DunIt.Core.Firebase;
using DunIt.Core.Models;
using DunIt.Testing;
using Moq;
using NUnit.Framework;
using Shouldly;

public class FirebaseChildRepositoryTests
{
    [Test, AutoMoqData]
    public async Task ShouldAddChild_WhenChildProvided(
        Child child,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChildRepository sut)
    {
        // Arrange
        firebaseInteropSpy.Setup(f => f.AddChild(It.IsAny<ChildDto>()))
            .ReturnsAsync(new ChildDto(child.Id, child.Name, child.Avatar));

        // Act
        var result = await sut.AddChild(child);

        // Assert
        firebaseInteropSpy.Verify(f => f.AddChild(
            It.Is<ChildDto>(d => d.Id == child.Id && d.Name == child.Name && d.Avatar == child.Avatar)),
            Times.Once);
        result.ShouldBe(child);
    }

    [Test, AutoMoqData]
    public async Task ShouldDeleteChild_WhenChildDeleted(
        string childId,
        [Frozen] Mock<IFirebaseInterop> firebaseInteropSpy,
        FirebaseChildRepository sut)
    {
        // Act
        await sut.DeleteChild(childId);

        // Assert
        firebaseInteropSpy.Verify(f => f.DeleteChild(childId), Times.Once);
    }

    [Test, AutoMoqData]
    public async Task ShouldReturnAllChildren_WhenChildrenExist(
        [Frozen] Mock<IFirebaseInterop> firebaseInteropStub,
        FirebaseChildRepository sut)
    {
        // Arrange
        firebaseInteropStub.Setup(f => f.GetChildren()).ReturnsAsync(
        [
            new ChildDto("child-1", "Alice", "👧"),
            new ChildDto("child-2", "Bob", "👦")
        ]);

        // Act
        var result = await sut.GetChildren();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Id.ShouldBe("child-1");
        result[0].Name.ShouldBe("Alice");
        result[1].Id.ShouldBe("child-2");
    }
}

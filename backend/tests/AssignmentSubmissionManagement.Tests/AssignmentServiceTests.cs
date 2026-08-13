using AssignmentSubmissionManagement.Core.DTOs.Assignments;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentAssertions;
using Moq;
using System.Timers;

namespace AssignmentSubmissionManagement.Tests;

public class AssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock = new();
    private readonly Mock<IClassRepository> _classRepoMock = new();

    private AssignmentService CreateSut() =>
        new(_assignmentRepoMock.Object, _classRepoMock.Object);

    private static Assignment BuildAssignment(Guid? teacherId = null, Guid? classId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(7),
            MaxMarks = 100,
            ClassId = classId ?? Guid.NewGuid(),
            TeacherId = teacherId ?? Guid.NewGuid(),
            Class = new Class { Id = classId ?? Guid.NewGuid(), Name = "Test Class", Description = "" },
            Teacher = new User { Id = teacherId ?? Guid.NewGuid(), FirstName = "T", LastName = "T", Email = "t@t.com" },
            Submissions = new List<Submission>()
        };

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WhenTeacherNotAssignedToClass_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        _classRepoMock.Setup(r => r.IsTeacherAssignedAsync(classId, teacherId)).ReturnsAsync(false);

        var sut = CreateSut();
        var request = new CreateAssignmentRequest
        {
            Title = "Assignment 1",
            Description = "Description",
            DueDate = DateTime.UtcNow.AddDays(7),
            MaxMarks = 100,
            ClassId = classId
        };

        // Act
        var act = () => sut.CreateAsync(request, teacherId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*not assigned*");
    }

    [Fact]
    public async Task CreateAsync_WhenTeacherAssigned_CreatesSuccessfully()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId, classId);

        _classRepoMock.Setup(r => r.IsTeacherAssignedAsync(classId, teacherId)).ReturnsAsync(true);
        _assignmentRepoMock.Setup(r => r.CreateAsync(It.IsAny<Assignment>())).ReturnsAsync(assignment);
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);

        var sut = CreateSut();
        var request = new CreateAssignmentRequest
        {
            Title = assignment.Title,
            Description = assignment.Description,
            DueDate = assignment.DueDate,
            MaxMarks = assignment.MaxMarks,
            ClassId = classId
        };

        // Act
        var result = await sut.CreateAsync(request, teacherId);

        // Assert
        result.Should().NotBeNull();
        result.TeacherId.Should().Be(teacherId);
        result.ClassId.Should().Be(classId);
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerTeacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: ownerTeacherId);

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);

        var sut = CreateSut();
        var request = new UpdateAssignmentRequest
        {
            Title = "New Title",
            Description = "New Desc",
            DueDate = DateTime.UtcNow.AddDays(14),
            MaxMarks = 50
        };

        // Act
        var act = () => sut.UpdateAsync(assignment.Id, request, otherTeacherId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*own assignments*");
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenSubmissionsExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: teacherId);

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _assignmentRepoMock.Setup(r => r.HasSubmissionsAsync(assignment.Id)).ReturnsAsync(true);

        var sut = CreateSut();

        // Act
        var act = () => sut.DeleteAsync(assignment.Id, teacherId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*submissions*");
    }

    [Fact]
    public async Task DeleteAsync_WhenNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerTeacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: ownerTeacherId);

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);

        var sut = CreateSut();

        // Act
        var act = () => sut.DeleteAsync(assignment.Id, otherTeacherId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Assignment?)null);

        var sut = CreateSut();

        // Act
        var act = () => sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenValid_CallsRepository()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: teacherId);

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _assignmentRepoMock.Setup(r => r.HasSubmissionsAsync(assignment.Id)).ReturnsAsync(false);
        _assignmentRepoMock.Setup(r => r.DeleteAsync(assignment.Id)).Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.DeleteAsync(assignment.Id, teacherId);

        // Assert
        _assignmentRepoMock.Verify(r => r.DeleteAsync(assignment.Id), Times.Once);
    }
}

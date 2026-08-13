using AssignmentSubmissionManagement.Core.DTOs.Classes;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentAssertions;
using Moq;

namespace AssignmentSubmissionManagement.Tests;

public class ClassServiceTests
{
    private readonly Mock<IClassRepository> _classRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private ClassService CreateSut() => new(_classRepoMock.Object, _userRepoMock.Object);

    private static User BuildUser(UserRole role, Guid? id = null) =>
        new()
        {
            Id = id ?? Guid.NewGuid(),
            FirstName = "First",
            LastName = "Last",
            Email = "user@test.com",
            Role = role
        };

    private static Class BuildClass(Guid? id = null) =>
        new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Test Class",
            Description = "Description",
            ClassTeachers = new List<ClassTeacher>(),
            ClassStudents = new List<ClassStudent>()
        };

    // ── AssignTeacherAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AssignTeacherAsync_WhenUserIsNotTeacher_ThrowsInvalidOperationException()
    {
        // Arrange
        var cls = BuildClass();
        var student = BuildUser(UserRole.Student);

        _classRepoMock.Setup(r => r.ExistsAsync(cls.Id)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(student.Id)).ReturnsAsync(student);

        var sut = CreateSut();
        var request = new AssignTeacherRequest { TeacherId = student.Id };

        // Act
        var act = () => sut.AssignTeacherAsync(cls.Id, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not a teacher*");
    }

    [Fact]
    public async Task AssignTeacherAsync_WhenAlreadyAssigned_ThrowsInvalidOperationException()
    {
        // Arrange
        var cls = BuildClass();
        var teacher = BuildUser(UserRole.Teacher);

        _classRepoMock.Setup(r => r.ExistsAsync(cls.Id)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(teacher.Id)).ReturnsAsync(teacher);
        _classRepoMock.Setup(r => r.IsTeacherAssignedAsync(cls.Id, teacher.Id)).ReturnsAsync(true);

        var sut = CreateSut();
        var request = new AssignTeacherRequest { TeacherId = teacher.Id };

        // Act
        var act = () => sut.AssignTeacherAsync(cls.Id, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already assigned*");
    }

    [Fact]
    public async Task AssignTeacherAsync_WhenValid_CallsRepository()
    {
        // Arrange
        var cls = BuildClass();
        var teacher = BuildUser(UserRole.Teacher);

        _classRepoMock.Setup(r => r.ExistsAsync(cls.Id)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(teacher.Id)).ReturnsAsync(teacher);
        _classRepoMock.Setup(r => r.IsTeacherAssignedAsync(cls.Id, teacher.Id)).ReturnsAsync(false);
        _classRepoMock.Setup(r => r.AssignTeacherAsync(cls.Id, teacher.Id)).Returns(Task.CompletedTask);

        var sut = CreateSut();
        var request = new AssignTeacherRequest { TeacherId = teacher.Id };

        // Act
        await sut.AssignTeacherAsync(cls.Id, request);

        // Assert
        _classRepoMock.Verify(r => r.AssignTeacherAsync(cls.Id, teacher.Id), Times.Once);
    }

    // ── EnrollStudentAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task EnrollStudentAsync_WhenUserIsNotStudent_ThrowsInvalidOperationException()
    {
        // Arrange
        var cls = BuildClass();
        var teacher = BuildUser(UserRole.Teacher);

        _classRepoMock.Setup(r => r.ExistsAsync(cls.Id)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(teacher.Id)).ReturnsAsync(teacher);

        var sut = CreateSut();
        var request = new EnrollStudentRequest { StudentId = teacher.Id };

        // Act
        var act = () => sut.EnrollStudentAsync(cls.Id, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not a student*");
    }

    [Fact]
    public async Task EnrollStudentAsync_WhenAlreadyEnrolled_ThrowsInvalidOperationException()
    {
        // Arrange
        var cls = BuildClass();
        var student = BuildUser(UserRole.Student);

        _classRepoMock.Setup(r => r.ExistsAsync(cls.Id)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(student.Id)).ReturnsAsync(student);
        _classRepoMock.Setup(r => r.IsStudentEnrolledAsync(cls.Id, student.Id)).ReturnsAsync(true);

        var sut = CreateSut();
        var request = new EnrollStudentRequest { StudentId = student.Id };

        // Act
        var act = () => sut.EnrollStudentAsync(cls.Id, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already enrolled*");
    }

    [Fact]
    public async Task EnrollStudentAsync_WhenValid_CallsRepository()
    {
        // Arrange
        var cls = BuildClass();
        var student = BuildUser(UserRole.Student);

        _classRepoMock.Setup(r => r.ExistsAsync(cls.Id)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(student.Id)).ReturnsAsync(student);
        _classRepoMock.Setup(r => r.IsStudentEnrolledAsync(cls.Id, student.Id)).ReturnsAsync(false);
        _classRepoMock.Setup(r => r.EnrollStudentAsync(cls.Id, student.Id)).Returns(Task.CompletedTask);

        var sut = CreateSut();
        var request = new EnrollStudentRequest { StudentId = student.Id };

        // Act
        await sut.EnrollStudentAsync(cls.Id, request);

        // Assert
        _classRepoMock.Verify(r => r.EnrollStudentAsync(cls.Id, student.Id), Times.Once);
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _classRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var act = () => sut.DeleteAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WhenValid_ReturnsClassResponse()
    {
        // Arrange
        var cls = BuildClass();

        _classRepoMock.Setup(r => r.CreateAsync(It.IsAny<Class>())).ReturnsAsync(cls);

        var sut = CreateSut();
        var request = new CreateClassRequest { Name = "Test Class", Description = "Desc" };

        // Act
        var result = await sut.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(cls.Name);
    }
}

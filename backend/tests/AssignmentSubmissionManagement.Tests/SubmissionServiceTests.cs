using AssignmentSubmissionManagement.Core.DTOs.Submissions;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentAssertions;
using Moq;

namespace AssignmentSubmissionManagement.Tests;

public class SubmissionServiceTests
{
    private readonly Mock<ISubmissionRepository> _submissionRepoMock = new();
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock = new();
    private readonly Mock<IClassRepository> _classRepoMock = new();
    private readonly Mock<IFileStorageService> _fileStorageMock = new();

    private SubmissionService CreateSut() =>
        new(_submissionRepoMock.Object,
            _assignmentRepoMock.Object,
            _classRepoMock.Object,
            _fileStorageMock.Object);

    private static Assignment BuildAssignment(
        Guid? teacherId = null,
        Guid? classId = null,
        DateTime? dueDate = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = "Test Assignment",
            Description = "Desc",
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(7),
            MaxMarks = 100,
            ClassId = classId ?? Guid.NewGuid(),
            TeacherId = teacherId ?? Guid.NewGuid(),
            Class = new Class { Id = classId ?? Guid.NewGuid(), Name = "Test Class", Description = "" },
            Teacher = new User { Id = teacherId ?? Guid.NewGuid(), FirstName = "T", LastName = "T", Email = "t@t.com" },
            Submissions = new List<Submission>()
        };

    private static Submission BuildSubmission(Guid assignmentId, Guid studentId, SubmissionStatus status = SubmissionStatus.Submitted) =>
        new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = studentId,
            Status = status,
            SubmittedAt = DateTime.UtcNow,
            Assignment = new Assignment
            {
                Id = assignmentId,
                Title = "Test",
                Description = "",
                DueDate = DateTime.UtcNow.AddDays(7),
                MaxMarks = 100,
                TeacherId = Guid.NewGuid()
            },
            Student = new User { Id = studentId, FirstName = "S", LastName = "S", Email = "s@s.com" }
        };

    // ── SubmitAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task SubmitAsync_WhenPastDueDate_ThrowsInvalidOperationException()
    {
        // Arrange
        var assignment = BuildAssignment(dueDate: DateTime.UtcNow.AddDays(-1));
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);

        var sut = CreateSut();
        var request = new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            TextContent = "Late submission"
        };

        // Act
        var act = () => sut.SubmitAsync(request, null, Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*deadline*");
    }

    [Fact]
    public async Task SubmitAsync_WhenStudentNotEnrolled_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var assignment = BuildAssignment();
        var studentId = Guid.NewGuid();

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _classRepoMock.Setup(r => r.IsStudentEnrolledAsync(assignment.ClassId, studentId)).ReturnsAsync(false);

        var sut = CreateSut();
        var request = new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            TextContent = "My answer"
        };

        // Act
        var act = () => sut.SubmitAsync(request, null, studentId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*enrolled*");
    }

    [Fact]
    public async Task SubmitAsync_WhenDuplicateSubmission_ThrowsInvalidOperationException()
    {
        // Arrange
        var assignment = BuildAssignment();
        var studentId = Guid.NewGuid();
        var existing = BuildSubmission(assignment.Id, studentId);

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _classRepoMock.Setup(r => r.IsStudentEnrolledAsync(assignment.ClassId, studentId)).ReturnsAsync(true);
        _submissionRepoMock.Setup(r => r.GetByAssignmentAndStudentAsync(assignment.Id, studentId)).ReturnsAsync(existing);

        var sut = CreateSut();
        var request = new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            TextContent = "Duplicate"
        };

        // Act
        var act = () => sut.SubmitAsync(request, null, studentId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already submitted*");
    }

    [Fact]
    public async Task SubmitAsync_WhenNoTextOrFile_ThrowsInvalidOperationException()
    {
        // Arrange
        var assignment = BuildAssignment();
        var studentId = Guid.NewGuid();

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _classRepoMock.Setup(r => r.IsStudentEnrolledAsync(assignment.ClassId, studentId)).ReturnsAsync(true);
        _submissionRepoMock.Setup(r => r.GetByAssignmentAndStudentAsync(assignment.Id, studentId)).ReturnsAsync((Submission?)null);

        var sut = CreateSut();
        var request = new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            TextContent = null   // no text, no file
        };

        // Act
        var act = () => sut.SubmitAsync(request, null, studentId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*text content or a file*");
    }

    [Fact]
    public async Task SubmitAsync_WhenValid_CreatesSubmission()
    {
        // Arrange
        var assignment = BuildAssignment();
        var studentId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();

        var createdSubmission = BuildSubmission(assignment.Id, studentId);
        createdSubmission.Id = expectedId;
        createdSubmission.TextContent = "My answer";

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _classRepoMock.Setup(r => r.IsStudentEnrolledAsync(assignment.ClassId, studentId)).ReturnsAsync(true);
        _submissionRepoMock.Setup(r => r.GetByAssignmentAndStudentAsync(assignment.Id, studentId)).ReturnsAsync((Submission?)null);
        _submissionRepoMock.Setup(r => r.CreateAsync(It.IsAny<Submission>())).ReturnsAsync(createdSubmission);
        _submissionRepoMock.Setup(r => r.GetByIdAsync(expectedId)).ReturnsAsync(createdSubmission);

        var sut = CreateSut();
        var request = new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            TextContent = "My answer"
        };

        // Act
        var result = await sut.SubmitAsync(request, null, studentId);

        // Assert
        result.Should().NotBeNull();
        result.StudentId.Should().Be(studentId);
        result.Status.Should().Be(SubmissionStatus.Submitted);
    }

    // ── GradeAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GradeAsync_WhenNotOwnerTeacher_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var realTeacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: realTeacherId);
        var submission = BuildSubmission(assignment.Id, Guid.NewGuid());
        submission.Assignment = assignment;

        _submissionRepoMock.Setup(r => r.GetByIdAsync(submission.Id)).ReturnsAsync(submission);
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);

        var sut = CreateSut();
        var request = new GradeSubmissionRequest { Marks = 80 };

        // Act
        var act = () => sut.GradeAsync(submission.Id, request, otherTeacherId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GradeAsync_WhenMarksExceedMax_ThrowsInvalidOperationException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: teacherId);
        assignment.MaxMarks = 100;
        var submission = BuildSubmission(assignment.Id, Guid.NewGuid());
        submission.Assignment = assignment;

        _submissionRepoMock.Setup(r => r.GetByIdAsync(submission.Id)).ReturnsAsync(submission);
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);

        var sut = CreateSut();
        var request = new GradeSubmissionRequest { Marks = 150 }; // exceeds max

        // Act
        var act = () => sut.GradeAsync(submission.Id, request, teacherId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Marks must be*");
    }

    [Fact]
    public async Task GradeAsync_WhenValid_SetsStatusToGraded()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var assignment = BuildAssignment(teacherId: teacherId);
        assignment.MaxMarks = 100;
        var studentId = Guid.NewGuid();
        var submission = BuildSubmission(assignment.Id, studentId);
        submission.Assignment = assignment;

        var gradedSubmission = BuildSubmission(assignment.Id, studentId, SubmissionStatus.Graded);
        gradedSubmission.Marks = 85;
        gradedSubmission.GradedAt = DateTime.UtcNow;
        gradedSubmission.Assignment = assignment;
        gradedSubmission.Student = submission.Student;

        _submissionRepoMock.Setup(r => r.GetByIdAsync(submission.Id)).ReturnsAsync(submission);
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _submissionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Submission>())).ReturnsAsync(gradedSubmission);
        _submissionRepoMock.Setup(r => r.GetByIdAsync(gradedSubmission.Id)).ReturnsAsync(gradedSubmission);

        var sut = CreateSut();
        var request = new GradeSubmissionRequest { Marks = 85, Feedback = "Great work!" };

        // Act
        var result = await sut.GradeAsync(submission.Id, request, teacherId);

        // Assert
        result.Status.Should().Be(SubmissionStatus.Graded);
        result.Marks.Should().Be(85);
    }
}

using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AssignmentSubmissionManagement.Tests;

public class FileStorageServiceTests : IDisposable
{
    private readonly string _tempFolder;
    private readonly string _uploadsRoot;

    public FileStorageServiceTests()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), "AsmTests_" + Guid.NewGuid());
        _uploadsRoot = Path.Combine(_tempFolder, "wwwroot", "uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempFolder))
        {
            Directory.Delete(_tempFolder, recursive: true);
        }
    }

    [Fact]
    public async Task SaveFileAsync_CreatesFileAndReturnsRelativePath()
    {
        // Arrange
        var service = new FileStorageService(_uploadsRoot);
        var assignmentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var mockFile = new Mock<IFormFile>();
        var content = "Hello world assignment submission";
        var fileName = "test_solution.txt";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((target, token) => stream.CopyToAsync(target, token));

        // Act
        var (filePath, returnedFileName) = await service.SaveFileAsync(mockFile.Object, assignmentId, studentId);

        // Assert
        returnedFileName.Should().Be(fileName);
        filePath.Should().StartWith($"uploads/{assignmentId}/");
        service.FileExists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task DeleteFileAsync_RemovesStoredFile()
    {
        // Arrange
        var service = new FileStorageService(_uploadsRoot);
        var assignmentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var mockFile = new Mock<IFormFile>();
        var content = "Delete test content";
        var fileName = "to_delete.txt";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((target, token) => stream.CopyToAsync(target, token));

        var (filePath, _) = await service.SaveFileAsync(mockFile.Object, assignmentId, studentId);
        service.FileExists(filePath).Should().BeTrue();

        // Act
        await service.DeleteFileAsync(filePath);

        // Assert
        service.FileExists(filePath).Should().BeFalse();
    }
}

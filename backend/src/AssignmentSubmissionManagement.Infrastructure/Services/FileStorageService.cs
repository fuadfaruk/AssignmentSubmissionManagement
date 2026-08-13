using AssignmentSubmissionManagement.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadsRoot;

        public FileStorageService(string uploadsRoot)
        {
            _uploadsRoot = uploadsRoot;
        }

        public async Task<(string filePath, string fileName)> SaveFileAsync(
            IFormFile file,
            Guid assignmentId,
            Guid studentId)
        {
            var assignmentDir = Path.Combine(_uploadsRoot, assignmentId.ToString());
            Directory.CreateDirectory(assignmentDir);

            var sanitizedOriginalName = Path.GetFileName(file.FileName);
            var uniqueName = $"{studentId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{sanitizedOriginalName}";
            var fullPath = Path.Combine(assignmentDir, uniqueName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine("uploads", assignmentId.ToString(), uniqueName)
                .Replace('\\', '/');

            return (relativePath, sanitizedOriginalName);
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_uploadsRoot, "..", filePath.TrimStart('/'));
            if (File.Exists(fullPath))
                await Task.Run(() => File.Delete(fullPath));
        }

        public bool FileExists(string filePath)
        {
            var fullPath = Path.Combine(_uploadsRoot, "..", filePath.TrimStart('/'));
            return File.Exists(fullPath);
        }
    }
}

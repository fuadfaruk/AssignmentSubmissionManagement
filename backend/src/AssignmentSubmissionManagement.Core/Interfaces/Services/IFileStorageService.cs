using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<(string filePath, string fileName)> SaveFileAsync(IFormFile file, Guid assignmentId, Guid studentId);
        Task DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
    }
}

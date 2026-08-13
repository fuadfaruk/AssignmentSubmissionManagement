using AssignmentSubmissionManagement.Core.DTOs.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Services
{
    public interface IClassService
    {
        Task<ClassResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<ClassResponse>> GetAllAsync();
        Task<IEnumerable<ClassResponse>> GetByTeacherIdAsync(Guid teacherId);
        Task<IEnumerable<ClassResponse>> GetByStudentIdAsync(Guid studentId);
        Task<ClassResponse> CreateAsync(CreateClassRequest request);
        Task<ClassResponse> UpdateAsync(Guid id, UpdateClassRequest request);
        Task DeleteAsync(Guid id);
        Task AssignTeacherAsync(Guid classId, AssignTeacherRequest request);
        Task RemoveTeacherAsync(Guid classId, Guid teacherId);
        Task EnrollStudentAsync(Guid classId, EnrollStudentRequest request);
        Task RemoveStudentAsync(Guid classId, Guid studentId);
    }
}

using AssignmentSubmissionManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Repositories
{
    public interface IClassRepository
    {
        Task<Class?> GetByIdAsync(Guid id);
        Task<IEnumerable<Class>> GetAllAsync();
        Task<IEnumerable<Class>> GetByTeacherIdAsync(Guid teacherId);
        Task<IEnumerable<Class>> GetByStudentIdAsync(Guid studentId);
        Task<Class> CreateAsync(Class @class);
        Task<Class> UpdateAsync(Class @class);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AssignTeacherAsync(Guid classId, Guid teacherId);
        Task RemoveTeacherAsync(Guid classId, Guid teacherId);
        Task EnrollStudentAsync(Guid classId, Guid studentId);
        Task RemoveStudentAsync(Guid classId, Guid studentId);
        Task<bool> IsTeacherAssignedAsync(Guid classId, Guid teacherId);
        Task<bool> IsStudentEnrolledAsync(Guid classId, Guid studentId);
    }
}

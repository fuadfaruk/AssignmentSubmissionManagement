using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Repositories
{
    public interface ISubmissionRepository
    {
        Task<Submission?> GetByIdAsync(Guid id);
        Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId);
        Task<PagedResult<Submission>> GetByAssignmentIdPagedAsync(Guid assignmentId, int page, int pageSize);
        Task<PagedResult<Submission>> GetByStudentIdPagedAsync(Guid studentId, int page, int pageSize);
        Task<Submission> CreateAsync(Submission submission);
        Task<Submission> UpdateAsync(Submission submission);
        Task<bool> ExistsAsync(Guid id);
        Task<int> CountByAssignmentIdAsync(Guid assignmentId);
    }
}

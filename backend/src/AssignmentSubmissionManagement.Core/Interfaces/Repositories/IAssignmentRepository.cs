using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Repositories
{
    public interface IAssignmentRepository
    {
        Task<Assignment?> GetByIdAsync(Guid id);
        Task<PagedResult<Assignment>> GetPagedAsync(
            int page,
            int pageSize,
            Guid? classId = null,
            Guid? teacherId = null,
            DateTime? dueBefore = null,
            DateTime? dueAfter = null);
        Task<IEnumerable<Assignment>> GetByClassIdsAsync(IEnumerable<Guid> classIds);
        Task<Assignment> CreateAsync(Assignment assignment);
        Task<Assignment> UpdateAsync(Assignment assignment);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasSubmissionsAsync(Guid assignmentId);
    }
}

using AssignmentSubmissionManagement.Core.DTOs.Assignments;
using AssignmentSubmissionManagement.Core.DTOs.Common;


namespace AssignmentSubmissionManagement.Infrastructure.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentResponse?> GetByIdAsync(Guid id);
        Task<PagedResult<AssignmentResponse>> GetPagedAsync(
            int page,
            int pageSize,
            Guid? classId = null,
            DateTime? dueBefore = null,
            DateTime? dueAfter = null,
            Guid? teacherId = null);
        Task<PagedResult<AssignmentResponse>> GetForStudentPagedAsync(
            Guid studentId,
            int page,
            int pageSize,
            Guid? classId = null,
            DateTime? dueBefore = null,
            DateTime? dueAfter = null);
        Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, Guid teacherId);
        Task<AssignmentResponse> UpdateAsync(Guid id, UpdateAssignmentRequest request, Guid teacherId);
        Task DeleteAsync(Guid id, Guid teacherId);
    }
}

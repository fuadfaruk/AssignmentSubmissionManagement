using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.DTOs.Submissions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Services
{
    public interface ISubmissionService
    {
        Task<SubmissionResponse?> GetByIdAsync(Guid id);
        Task<PagedResult<SubmissionResponse>> GetByAssignmentIdAsync(Guid assignmentId, int page, int pageSize, Guid teacherId);
        Task<PagedResult<SubmissionResponse>> GetByStudentIdAsync(Guid studentId, int page, int pageSize);
        Task<SubmissionResponse> SubmitAsync(CreateSubmissionRequest request, IFormFile? file, Guid studentId);
        Task<SubmissionResponse> GradeAsync(Guid submissionId, GradeSubmissionRequest request, Guid teacherId);
    }
}

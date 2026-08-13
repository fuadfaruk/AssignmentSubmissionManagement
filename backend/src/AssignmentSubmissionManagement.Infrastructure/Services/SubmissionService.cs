using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.DTOs.Submissions;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace AssignmentSubmissionManagement.Infrastructure.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IFileStorageService _fileStorageService;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            IAssignmentRepository assignmentRepository,
            IClassRepository classRepository,
            IFileStorageService fileStorageService)
        {
            _submissionRepository = submissionRepository;
            _assignmentRepository = assignmentRepository;
            _classRepository = classRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<SubmissionResponse?> GetByIdAsync(Guid id)
        {
            var submission = await _submissionRepository.GetByIdAsync(id);
            return submission is null ? null : MapToResponse(submission);
        }

        public async Task<PagedResult<SubmissionResponse>> GetByAssignmentIdAsync(
            Guid assignmentId,
            int page,
            int pageSize,
            Guid teacherId)
        {
            // Verify this assignment belongs to the teacher
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId)
                ?? throw new KeyNotFoundException($"Assignment with id '{assignmentId}' not found.");

            if (assignment.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You can only view submissions for your own assignments.");

            var paged = await _submissionRepository.GetByAssignmentIdPagedAsync(assignmentId, page, pageSize);
            return new PagedResult<SubmissionResponse>
            {
                Items = paged.Items.Select(MapToResponse).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<PagedResult<SubmissionResponse>> GetByStudentIdAsync(Guid studentId, int page, int pageSize)
        {
            var paged = await _submissionRepository.GetByStudentIdPagedAsync(studentId, page, pageSize);
            return new PagedResult<SubmissionResponse>
            {
                Items = paged.Items.Select(MapToResponse).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<SubmissionResponse> SubmitAsync(CreateSubmissionRequest request, IFormFile? file, Guid studentId)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId)
                ?? throw new KeyNotFoundException($"Assignment with id '{request.AssignmentId}' not found.");

            // Cannot submit after due date
            if (DateTime.UtcNow > assignment.DueDate)
                throw new InvalidOperationException("The submission deadline has passed.");

            // Student must be enrolled in the class
            if (!await _classRepository.IsStudentEnrolledAsync(assignment.ClassId, studentId))
                throw new UnauthorizedAccessException("You are not enrolled in the class for this assignment.");

            // Prevent duplicate submissions
            var existing = await _submissionRepository.GetByAssignmentAndStudentAsync(request.AssignmentId, studentId);
            if (existing is not null)
                throw new InvalidOperationException("You have already submitted this assignment.");

            // Must provide text or file
            if (string.IsNullOrWhiteSpace(request.TextContent) && file is null)
                throw new InvalidOperationException("Submission must include text content or a file.");

            string? filePath = null;
            string? fileName = null;

            if (file is not null)
                (filePath, fileName) = await _fileStorageService.SaveFileAsync(file, request.AssignmentId, studentId);

            var submission = new Submission
            {
                AssignmentId = request.AssignmentId,
                StudentId = studentId,
                TextContent = request.TextContent,
                FilePath = filePath,
                FileName = fileName,
                Status = SubmissionStatus.Submitted,
                SubmittedAt = DateTime.UtcNow
            };

            var created = await _submissionRepository.CreateAsync(submission);
            var full = await _submissionRepository.GetByIdAsync(created.Id);
            return MapToResponse(full!);
        }

        public async Task<SubmissionResponse> GradeAsync(Guid submissionId, GradeSubmissionRequest request, Guid teacherId)
        {
            var submission = await _submissionRepository.GetByIdAsync(submissionId)
                ?? throw new KeyNotFoundException($"Submission with id '{submissionId}' not found.");

            // Only the teacher who owns the assignment can grade
            var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId)
                ?? throw new KeyNotFoundException("Associated assignment not found.");

            if (assignment.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You can only grade submissions for your own assignments.");

            if (request.Marks < 0 || request.Marks > assignment.MaxMarks)
                throw new InvalidOperationException(
                    $"Marks must be between 0 and {assignment.MaxMarks}.");

            submission.Marks = request.Marks;
            submission.Feedback = request.Feedback;
            submission.Status = SubmissionStatus.Graded;
            submission.GradedAt = DateTime.UtcNow;

            var updated = await _submissionRepository.UpdateAsync(submission);
            var full = await _submissionRepository.GetByIdAsync(updated.Id);
            return MapToResponse(full!);
        }

        private static SubmissionResponse MapToResponse(Submission s) => new()
        {
            Id = s.Id,
            AssignmentId = s.AssignmentId,
            AssignmentTitle = s.Assignment?.Title ?? string.Empty,
            StudentId = s.StudentId,
            StudentName = s.Student is not null
                ? $"{s.Student.FirstName} {s.Student.LastName}"
                : string.Empty,
            TextContent = s.TextContent,
            FileName = s.FileName,
            FilePath = s.FilePath,
            Status = s.Status,
            Marks = s.Marks,
            Feedback = s.Feedback,
            SubmittedAt = s.SubmittedAt,
            GradedAt = s.GradedAt
        };
    }

}

using AssignmentSubmissionManagement.Core.DTOs.Assignments;
using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IClassRepository _classRepository;

        public AssignmentService(
            IAssignmentRepository assignmentRepository,
            IClassRepository classRepository)
        {
            _assignmentRepository = assignmentRepository;
            _classRepository = classRepository;
        }

        public async Task<AssignmentResponse?> GetByIdAsync(Guid id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            return assignment is null ? null : MapToResponse(assignment);
        }

        public async Task<PagedResult<AssignmentResponse>> GetPagedAsync(
            int page,
            int pageSize,
            Guid? classId = null,
            DateTime? dueBefore = null,
            DateTime? dueAfter = null,
            Guid? teacherId = null)
        {
            var paged = await _assignmentRepository.GetPagedAsync(page, pageSize, classId, teacherId, dueBefore, dueAfter);
            return new PagedResult<AssignmentResponse>
            {
                Items = paged.Items.Select(MapToResponse).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<PagedResult<AssignmentResponse>> GetForStudentPagedAsync(
            Guid studentId,
            int page,
            int pageSize,
            Guid? classId = null,
            DateTime? dueBefore = null,
            DateTime? dueAfter = null)
        {
            var studentClasses = await _classRepository.GetByStudentIdAsync(studentId);
            var classIds = studentClasses.Select(c => c.Id).ToList();

            if (classId.HasValue)
                classIds = classIds.Where(id => id == classId.Value).ToList();

            if (!classIds.Any())
                return new PagedResult<AssignmentResponse> { Page = page, PageSize = pageSize };

            var all = (await _assignmentRepository.GetByClassIdsAsync(classIds)).ToList();

            if (dueBefore.HasValue)
                all = all.Where(a => a.DueDate <= dueBefore.Value).ToList();
            if (dueAfter.HasValue)
                all = all.Where(a => a.DueDate >= dueAfter.Value).ToList();

            var total = all.Count;
            var items = all
                .OrderByDescending(a => a.DueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToResponse)
                .ToList();

            return new PagedResult<AssignmentResponse>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, Guid teacherId)
        {
            if (!await _classRepository.IsTeacherAssignedAsync(request.ClassId, teacherId))
                throw new UnauthorizedAccessException("You are not assigned to this class.");

            var assignment = new Assignment
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                MaxMarks = request.MaxMarks,
                ClassId = request.ClassId,
                TeacherId = teacherId
            };

            var created = await _assignmentRepository.CreateAsync(assignment);

            var full = await _assignmentRepository.GetByIdAsync(created.Id);
            return MapToResponse(full!);
        }

        public async Task<AssignmentResponse> UpdateAsync(Guid id, UpdateAssignmentRequest request, Guid teacherId)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Assignment with id '{id}' not found.");

            if (assignment.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You can only update your own assignments.");

            assignment.Title = request.Title;
            assignment.Description = request.Description;
            assignment.DueDate = request.DueDate;
            assignment.MaxMarks = request.MaxMarks;

            var updated = await _assignmentRepository.UpdateAsync(assignment);
            var full = await _assignmentRepository.GetByIdAsync(updated.Id);
            return MapToResponse(full!);
        }

        public async Task DeleteAsync(Guid id, Guid teacherId)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Assignment with id '{id}' not found.");

            if (assignment.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You can only delete your own assignments.");

            if (await _assignmentRepository.HasSubmissionsAsync(id))
                throw new InvalidOperationException("Cannot delete an assignment that already has submissions.");

            await _assignmentRepository.DeleteAsync(id);
        }

        private static AssignmentResponse MapToResponse(Assignment a) => new()
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            DueDate = a.DueDate,
            MaxMarks = a.MaxMarks,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            ClassId = a.ClassId,
            ClassName = a.Class?.Name ?? string.Empty,
            TeacherId = a.TeacherId,
            TeacherName = a.Teacher is not null
                ? $"{a.Teacher.FirstName} {a.Teacher.LastName}"
                : string.Empty,
            SubmissionCount = a.Submissions?.Count ?? 0
        };
    }
}

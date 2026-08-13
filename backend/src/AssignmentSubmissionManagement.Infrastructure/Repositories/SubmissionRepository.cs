using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Repositories
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubmissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Submission?> GetByIdAsync(Guid id) =>
            await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId) =>
            await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

        public async Task<PagedResult<Submission>> GetByAssignmentIdPagedAsync(Guid assignmentId, int page, int pageSize)
        {
            var query = _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.AssignmentId == assignmentId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.SubmittedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Submission>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Submission>> GetByStudentIdPagedAsync(Guid studentId, int page, int pageSize)
        {
            var query = _context.Submissions
                .Include(s => s.Assignment).ThenInclude(a => a.Class)
                .Include(s => s.Student)
                .Where(s => s.StudentId == studentId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.SubmittedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Submission>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Submission> CreateAsync(Submission submission)
        {
            submission.Id = Guid.NewGuid();
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<Submission> UpdateAsync(Submission submission)
        {
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Submissions.AnyAsync(s => s.Id == id);

        public async Task<int> CountByAssignmentIdAsync(Guid assignmentId) =>
            await _context.Submissions.CountAsync(s => s.AssignmentId == assignmentId);
    }
}

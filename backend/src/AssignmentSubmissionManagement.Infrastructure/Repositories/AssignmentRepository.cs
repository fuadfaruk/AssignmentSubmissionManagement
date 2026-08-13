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
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Assignment?> GetByIdAsync(Guid id) =>
            await _context.Assignments
                .Include(a => a.Class)
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<PagedResult<Assignment>> GetPagedAsync(
            int page,
            int pageSize,
            Guid? classId = null,
            Guid? teacherId = null,
            DateTime? dueBefore = null,
            DateTime? dueAfter = null)
        {
            var query = _context.Assignments
                .Include(a => a.Class)
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                .AsQueryable();

            if (classId.HasValue)
                query = query.Where(a => a.ClassId == classId.Value);
            if (teacherId.HasValue)
                query = query.Where(a => a.TeacherId == teacherId.Value);
            if (dueBefore.HasValue)
                query = query.Where(a => a.DueDate <= dueBefore.Value);
            if (dueAfter.HasValue)
                query = query.Where(a => a.DueDate >= dueAfter.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Assignment>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Assignment>> GetByClassIdsAsync(IEnumerable<Guid> classIds) =>
            await _context.Assignments
                .Include(a => a.Class)
                .Include(a => a.Teacher)
                .Where(a => classIds.Contains(a.ClassId))
                .OrderByDescending(a => a.DueDate)
                .ToListAsync();

        public async Task<Assignment> CreateAsync(Assignment assignment)
        {
            assignment.Id = Guid.NewGuid();
            assignment.CreatedAt = DateTime.UtcNow;
            assignment.UpdatedAt = DateTime.UtcNow;
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAsync(Assignment assignment)
        {
            assignment.UpdatedAt = DateTime.UtcNow;
            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task DeleteAsync(Guid id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment is not null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Assignments.AnyAsync(a => a.Id == id);

        public async Task<bool> HasSubmissionsAsync(Guid assignmentId) =>
            await _context.Submissions.AnyAsync(s => s.AssignmentId == assignmentId);
    }
}

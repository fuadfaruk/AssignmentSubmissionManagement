using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Class?> GetByIdAsync(Guid id) =>
            await _context.Classes
                .Include(c => c.ClassTeachers).ThenInclude(ct => ct.Teacher)
                .Include(c => c.ClassStudents).ThenInclude(cs => cs.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Class>> GetAllAsync() =>
            await _context.Classes
                .Include(c => c.ClassTeachers).ThenInclude(ct => ct.Teacher)
                .Include(c => c.ClassStudents).ThenInclude(cs => cs.Student)
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<IEnumerable<Class>> GetByTeacherIdAsync(Guid teacherId) =>
            await _context.Classes
                .Include(c => c.ClassTeachers).ThenInclude(ct => ct.Teacher)
                .Include(c => c.ClassStudents).ThenInclude(cs => cs.Student)
                .Where(c => c.ClassTeachers.Any(ct => ct.TeacherId == teacherId))
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<IEnumerable<Class>> GetByStudentIdAsync(Guid studentId) =>
            await _context.Classes
                .Include(c => c.ClassTeachers).ThenInclude(ct => ct.Teacher)
                .Include(c => c.ClassStudents).ThenInclude(cs => cs.Student)
                .Where(c => c.ClassStudents.Any(cs => cs.StudentId == studentId))
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<Class> CreateAsync(Class @class)
        {
            @class.Id = Guid.NewGuid();
            @class.CreatedAt = DateTime.UtcNow;
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();
            return @class;
        }

        public async Task<Class> UpdateAsync(Class @class)
        {
            _context.Classes.Update(@class);
            await _context.SaveChangesAsync();
            return @class;
        }

        public async Task DeleteAsync(Guid id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class is not null)
            {
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Classes.AnyAsync(c => c.Id == id);

        public async Task AssignTeacherAsync(Guid classId, Guid teacherId)
        {
            var ct = new ClassTeacher { ClassId = classId, TeacherId = teacherId, AssignedAt = DateTime.UtcNow };
            _context.ClassTeachers.Add(ct);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTeacherAsync(Guid classId, Guid teacherId)
        {
            var ct = await _context.ClassTeachers
                .FirstOrDefaultAsync(x => x.ClassId == classId && x.TeacherId == teacherId);
            if (ct is not null)
            {
                _context.ClassTeachers.Remove(ct);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EnrollStudentAsync(Guid classId, Guid studentId)
        {
            var cs = new ClassStudent { ClassId = classId, StudentId = studentId, EnrolledAt = DateTime.UtcNow };
            _context.ClassStudents.Add(cs);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveStudentAsync(Guid classId, Guid studentId)
        {
            var cs = await _context.ClassStudents
                .FirstOrDefaultAsync(x => x.ClassId == classId && x.StudentId == studentId);
            if (cs is not null)
            {
                _context.ClassStudents.Remove(cs);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTeacherAssignedAsync(Guid classId, Guid teacherId) =>
            await _context.ClassTeachers.AnyAsync(ct => ct.ClassId == classId && ct.TeacherId == teacherId);

        public async Task<bool> IsStudentEnrolledAsync(Guid classId, Guid studentId) =>
            await _context.ClassStudents.AnyAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);

    }
}

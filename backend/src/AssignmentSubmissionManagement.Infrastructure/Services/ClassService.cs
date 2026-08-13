using AssignmentSubmissionManagement.Core.DTOs.Classes;
using AssignmentSubmissionManagement.Core.DTOs.Users;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IUserRepository _userRepository;

        public ClassService(IClassRepository classRepository, IUserRepository userRepository)
        {
            _classRepository = classRepository;
            _userRepository = userRepository;
        }

        public async Task<ClassResponse?> GetByIdAsync(Guid id)
        {
            var cls = await _classRepository.GetByIdAsync(id);
            return cls is null ? null : MapToResponse(cls);
        }

        public async Task<IEnumerable<ClassResponse>> GetAllAsync()
        {
            var classes = await _classRepository.GetAllAsync();
            return classes.Select(MapToResponse);
        }

        public async Task<IEnumerable<ClassResponse>> GetByTeacherIdAsync(Guid teacherId)
        {
            var classes = await _classRepository.GetByTeacherIdAsync(teacherId);
            return classes.Select(MapToResponse);
        }

        public async Task<IEnumerable<ClassResponse>> GetByStudentIdAsync(Guid studentId)
        {
            var classes = await _classRepository.GetByStudentIdAsync(studentId);
            return classes.Select(MapToResponse);
        }

        public async Task<ClassResponse> CreateAsync(CreateClassRequest request)
        {
            var cls = new Class
            {
                Name = request.Name,
                Description = request.Description
            };

            var created = await _classRepository.CreateAsync(cls);
            return MapToResponse(created);
        }

        public async Task<ClassResponse> UpdateAsync(Guid id, UpdateClassRequest request)
        {
            var cls = await _classRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Class with id '{id}' not found.");

            cls.Name = request.Name;
            cls.Description = request.Description;

            var updated = await _classRepository.UpdateAsync(cls);
            return MapToResponse(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _classRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Class with id '{id}' not found.");

            await _classRepository.DeleteAsync(id);
        }

        public async Task AssignTeacherAsync(Guid classId, AssignTeacherRequest request)
        {
            if (!await _classRepository.ExistsAsync(classId))
                throw new KeyNotFoundException($"Class with id '{classId}' not found.");

            var teacher = await _userRepository.GetByIdAsync(request.TeacherId)
                ?? throw new KeyNotFoundException($"User with id '{request.TeacherId}' not found.");

            if (teacher.Role != UserRole.Teacher)
                throw new InvalidOperationException("The specified user is not a teacher.");

            if (await _classRepository.IsTeacherAssignedAsync(classId, request.TeacherId))
                throw new InvalidOperationException("Teacher is already assigned to this class.");

            await _classRepository.AssignTeacherAsync(classId, request.TeacherId);
        }

        public async Task RemoveTeacherAsync(Guid classId, Guid teacherId)
        {
            if (!await _classRepository.ExistsAsync(classId))
                throw new KeyNotFoundException($"Class with id '{classId}' not found.");

            await _classRepository.RemoveTeacherAsync(classId, teacherId);
        }

        public async Task EnrollStudentAsync(Guid classId, EnrollStudentRequest request)
        {
            if (!await _classRepository.ExistsAsync(classId))
                throw new KeyNotFoundException($"Class with id '{classId}' not found.");

            var student = await _userRepository.GetByIdAsync(request.StudentId)
                ?? throw new KeyNotFoundException($"User with id '{request.StudentId}' not found.");

            if (student.Role != UserRole.Student)
                throw new InvalidOperationException("The specified user is not a student.");

            if (await _classRepository.IsStudentEnrolledAsync(classId, request.StudentId))
                throw new InvalidOperationException("Student is already enrolled in this class.");

            await _classRepository.EnrollStudentAsync(classId, request.StudentId);
        }

        public async Task RemoveStudentAsync(Guid classId, Guid studentId)
        {
            if (!await _classRepository.ExistsAsync(classId))
                throw new KeyNotFoundException($"Class with id '{classId}' not found.");

            await _classRepository.RemoveStudentAsync(classId, studentId);
        }

        private static ClassResponse MapToResponse(Class cls) => new()
        {
            Id = cls.Id,
            Name = cls.Name,
            Description = cls.Description,
            CreatedAt = cls.CreatedAt,
            Teachers = cls.ClassTeachers.Select(ct => new UserResponse
            {
                Id = ct.Teacher.Id,
                FirstName = ct.Teacher.FirstName,
                LastName = ct.Teacher.LastName,
                Email = ct.Teacher.Email,
                Role = ct.Teacher.Role,
                CreatedAt = ct.Teacher.CreatedAt
            }).ToList(),
            Students = cls.ClassStudents.Select(cs => new UserResponse
            {
                Id = cs.Student.Id,
                FirstName = cs.Student.FirstName,
                LastName = cs.Student.LastName,
                Email = cs.Student.Email,
                Role = cs.Student.Role,
                CreatedAt = cs.Student.CreatedAt
            }).ToList()
        };
    }

}

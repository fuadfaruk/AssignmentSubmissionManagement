using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }

            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Database already seeded. Skipping.");
                return;
            }

            logger.LogInformation("Seeding database...");

            var admin1 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Sarah",
                LastName = "Connor",
                Email = "admin@school.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            var admin2 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Admin",
                Email = "admin@demo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            var teacher1 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alan",
                LastName = "Turing",
                Email = "turing@school.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = UserRole.Teacher,
                CreatedAt = DateTime.UtcNow
            };

            var teacher2 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Margaret",
                LastName = "Hamilton",
                Email = "margaret@school.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = UserRole.Teacher,
                CreatedAt = DateTime.UtcNow
            };

            var teacher3 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Tom",
                LastName = "Teacher",
                Email = "teacher@demo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = UserRole.Teacher,
                CreatedAt = DateTime.UtcNow
            };

            var student1 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Johnson",
                Email = "alex@student.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student,
                CreatedAt = DateTime.UtcNow
            };

            var student2 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Sam",
                LastName = "Student",
                Email = "student@demo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddRangeAsync(admin1, admin2, teacher1, teacher2, teacher3, student1, student2);
            await context.SaveChangesAsync();

            var demoClass = new Class
            {
                Id = Guid.NewGuid(),
                Name = "Introduction to Computer Science",
                Description = "Covers fundamental programming concepts.",
                CreatedAt = DateTime.UtcNow
            };

            await context.Classes.AddAsync(demoClass);
            await context.SaveChangesAsync();

            var classTeacher = new ClassTeacher
            {
                ClassId = demoClass.Id,
                TeacherId = teacher1.Id,
                AssignedAt = DateTime.UtcNow
            };

            var classStudent = new ClassStudent
            {
                ClassId = demoClass.Id,
                StudentId = student1.Id,
                EnrolledAt = DateTime.UtcNow
            };

            await context.ClassTeachers.AddAsync(classTeacher);
            await context.ClassStudents.AddAsync(classStudent);
            await context.SaveChangesAsync();

            var assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                Title = "Hello World Program",
                Description = "Write a program that prints 'Hello, World!' in any programming language.",
                DueDate = DateTime.UtcNow.AddDays(7),
                MaxMarks = 100,
                ClassId = demoClass.Id,
                TeacherId = teacher1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Assignments.AddAsync(assignment);
            await context.SaveChangesAsync();

            logger.LogInformation(
                "Seeding complete. Demo accounts ready.");
        }

    }
}

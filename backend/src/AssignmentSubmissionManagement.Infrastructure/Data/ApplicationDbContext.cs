using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace AssignmentSubmissionManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Class> Classes => Set<Class>();
        public DbSet<ClassTeacher> ClassTeachers => Set<ClassTeacher>();
        public DbSet<ClassStudent> ClassStudents => Set<ClassStudent>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Submission> Submissions => Set<Submission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ClassConfiguration());
            modelBuilder.ApplyConfiguration(new ClassTeacherConfiguration());
            modelBuilder.ApplyConfiguration(new ClassStudentConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        }
    }
}

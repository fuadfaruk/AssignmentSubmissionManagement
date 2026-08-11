using AssignmentSubmissionManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Data.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Description).HasMaxLength(1000);

            builder.ToTable("classes");
        }
    }

    public class ClassTeacherConfiguration : IEntityTypeConfiguration<ClassTeacher>
    {
        public void Configure(EntityTypeBuilder<ClassTeacher> builder)
        {
            builder.HasKey(ct => new { ct.ClassId, ct.TeacherId });

            builder.HasOne(ct => ct.Class)
                   .WithMany(c => c.ClassTeachers)
                   .HasForeignKey(ct => ct.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ct => ct.Teacher)
                   .WithMany(u => u.ClassTeachers)
                   .HasForeignKey(ct => ct.TeacherId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("class_teachers");
        }
    }

    public class ClassStudentConfiguration : IEntityTypeConfiguration<ClassStudent>
    {
        public void Configure(EntityTypeBuilder<ClassStudent> builder)
        {
            builder.HasKey(cs => new { cs.ClassId, cs.StudentId });

            builder.HasOne(cs => cs.Class)
                   .WithMany(c => c.ClassStudents)
                   .HasForeignKey(cs => cs.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Student)
                   .WithMany(u => u.ClassStudents)
                   .HasForeignKey(cs => cs.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("class_students");
        }
    }
}

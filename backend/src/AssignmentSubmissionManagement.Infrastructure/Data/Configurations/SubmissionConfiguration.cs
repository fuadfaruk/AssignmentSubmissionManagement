using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Data.Configurations
{
    public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.Property(s => s.Status).HasConversion<int>();
            builder.Property(s => s.TextContent).HasMaxLength(10000);
            builder.Property(s => s.FilePath).HasMaxLength(500);
            builder.Property(s => s.FileName).HasMaxLength(255);
            builder.Property(s => s.Feedback).HasMaxLength(2000);

            builder.HasOne(s => s.Assignment)
                   .WithMany(a => a.Submissions)
                   .HasForeignKey(s => s.AssignmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Student)
                   .WithMany(u => u.Submissions)
                   .HasForeignKey(s => s.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Ensure that a student can only have one submission per assignment
            builder.HasIndex(s => new { s.AssignmentId, s.StudentId }).IsUnique();

            builder.ToTable("submissions");
        }
    }
}

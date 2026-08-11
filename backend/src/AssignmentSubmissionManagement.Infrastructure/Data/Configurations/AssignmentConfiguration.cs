using AssignmentSubmissionManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Data.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.Property(a => a.Title).IsRequired().HasMaxLength(300);
            builder.Property(a => a.Description).IsRequired().HasMaxLength(5000);
            builder.Property(a => a.DueDate).IsRequired();
            builder.Property(a => a.MaxMarks).IsRequired();

            builder.HasOne(a => a.Class)
                   .WithMany(c => c.Assignments)
                   .HasForeignKey(a => a.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Teacher)
                   .WithMany(t => t.Assignments)
                   .HasForeignKey(a => a.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("assignments");
        }
    }
}

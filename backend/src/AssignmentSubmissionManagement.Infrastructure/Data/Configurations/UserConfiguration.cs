using AssignmentSubmissionManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssignmentSubmissionManagement.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Role).IsRequired().HasConversion<int>();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.ToTable("users");
        }
    }
}

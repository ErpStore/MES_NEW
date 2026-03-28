using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MES.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // 2. Primary Key
            builder.HasKey(u => u.Id);

            // 3. Properties Configuration
            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .HasMaxLength(250);

            builder.Property(u => u.FirstName)
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .HasMaxLength(100);

            builder.Property(u => u.Mobile)
                .HasMaxLength(20);

            // 4. Indexes (Uniqueness & Performance)
            builder.HasIndex(u => u.UserName)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            // 5. Relationships
            // "One UserGroup has Many Users"
            builder.HasOne(u => u.UserGroup)
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.UserGroupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

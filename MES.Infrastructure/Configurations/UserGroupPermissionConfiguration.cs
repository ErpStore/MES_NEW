using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MES.Infrastructure.Configurations
{
    public class UserGroupPermissionConfiguration : IEntityTypeConfiguration<UserGroupPermission>
    {
        public void Configure(EntityTypeBuilder<UserGroupPermission> builder)
        {
            builder.ToTable("UserGroupPermissions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ScreenKey).IsRequired().HasMaxLength(100);

            // Default values
            builder.Property(x => x.CanAdd).HasDefaultValue(false);
            builder.Property(x => x.CanEdit).HasDefaultValue(false);
            builder.Property(x => x.CanDelete).HasDefaultValue(false);
            builder.Property(x => x.IsVisibleForUser).HasDefaultValue(false);

            builder.HasOne(x => x.UserGroup)
                   .WithMany()
                   .HasForeignKey(x => x.UserGroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserGroupId, x.ScreenKey }).IsUnique();
        }
    }
}
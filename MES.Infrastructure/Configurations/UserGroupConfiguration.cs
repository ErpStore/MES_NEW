using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        // 1. Table Name
        builder.ToTable("UserGroups");

        // 2. Primary Key
        builder.HasKey(g => g.Id);

        // 3. Properties Configuration
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100); // e.g. "Production", "Quality"

        builder.Property(g => g.Description)
            .HasMaxLength(250);

        // 4. Indexes
        // Prevent duplicate Department/Group names
        builder.HasIndex(g => g.Name)
            .IsUnique();
    }
}
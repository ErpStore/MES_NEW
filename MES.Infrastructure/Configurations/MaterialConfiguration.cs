using MES.Domain.Entities.Materials;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        // 1. Table & Key
        builder.ToTable("Materials");
        builder.HasKey(x => x.Id);

        // 2. Core Identity
        builder.Property(x => x.MaterialCode)
            .IsRequired()
            .HasMaxLength(50); // e.g. "RM-101"

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        // 3. New Fields Configuration

        // Unit is crucial for manufacturing (kg, L, pcs)
        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.HandlingInfo)
            .HasMaxLength(100); // e.g. "Keep Dry", "Flammable"

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(200);

        // Numeric Fields (Optional: You can define precision here if needed)
        // builder.Property(x => x.Density).HasPrecision(18, 4); 

        // 4. Description
        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // 5. Indexes
        // Material Code must be unique across the factory
        builder.HasIndex(x => x.MaterialCode).IsUnique();

        // 6. Relationships
        builder.HasOne(m => m.MaterialGroup)
            .WithMany(g => g.Materials)
            .HasForeignKey(m => m.MaterialGroupId)
            .OnDelete(DeleteBehavior.Restrict); // SAFETY: Prevent deleting a Group if it has Materials
    }
}
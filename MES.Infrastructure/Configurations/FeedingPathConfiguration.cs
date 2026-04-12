using MES.Domain.Entities.Materials;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations;

public class FeedingPathConfiguration : IEntityTypeConfiguration<FeedingPath>
{
    public void Configure(EntityTypeBuilder<FeedingPath> builder)
    {
        builder.ToTable("FeedingPaths");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BinNumber).IsRequired().HasMaxLength(20);
        builder.Property(x => x.BinCode).IsRequired().HasMaxLength(50);

        // UNIQUE INDEX: Bin Code must be unique
        builder.HasIndex(x => x.BinCode).IsUnique();

        builder.Property(x => x.Description).HasMaxLength(500);

        // RELATIONSHIP: Link to Material
        builder.HasOne(f => f.Material)
            .WithMany()
            .HasForeignKey(f => f.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
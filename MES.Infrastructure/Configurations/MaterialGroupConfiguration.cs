using MES.Domain.Entities.Materials;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations
{
    public class MaterialGroupConfiguration : IEntityTypeConfiguration<MaterialGroup>
    {
        public void Configure(EntityTypeBuilder<MaterialGroup> builder)
        {
            builder.ToTable("MaterialGroups");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            // UNIQUE INDEX: No two groups can have the same name
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}

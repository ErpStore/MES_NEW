using MES.Domain.Entities.Recipes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable("Recipes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RecipeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.RecipeCode).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Description).HasMaxLength(500);

            // UNIQUE INDEX: Recipe Code must be unique
            builder.HasIndex(x => x.RecipeCode).IsUnique();
        }
    }
}

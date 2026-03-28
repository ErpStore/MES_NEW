using MES.Domain.Entities.Recipes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations
{
    public class RecipeItemConfiguration : IEntityTypeConfiguration<RecipeItem>
    {
        public void Configure(EntityTypeBuilder<RecipeItem> builder)
        {
            builder.ToTable("RecipeItems");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(500);

            // RELATIONSHIP 1: If a Recipe is deleted, delete all its items (Cascade)
            builder.HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeItems)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // RELATIONSHIP 2: Prevent deleting a Material if it is used in a Recipe Item (Restrict)
            builder.HasOne(ri => ri.Material)
                .WithMany()
                .HasForeignKey(ri => ri.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

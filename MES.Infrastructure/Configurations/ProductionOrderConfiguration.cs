using MES.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Configurations
{
    public class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
    {
        public void Configure(EntityTypeBuilder<ProductionOrder> builder)
        {
            builder.ToTable("ProductionOrders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(500);

            // RELATIONSHIP: Prevent deleting a Recipe if an Order is using it
            builder.HasOne(o => o.Recipe)
                .WithMany()
                .HasForeignKey(o => o.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

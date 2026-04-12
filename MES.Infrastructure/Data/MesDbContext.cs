using MES.Domain.Entities;
using MES.Domain.Entities.Materials;
using MES.Domain.Entities.Orders;
using MES.Domain.Entities.Recipes;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Data;

public class MesDbContext : DbContext
{
    // 1. Define the Table
    public DbSet<User> Users { get; set; }

    public DbSet<UserGroup> UserGroups { get; set; } // Add this

    public DbSet<UserGroupRight> UserGroupRights { get; set; }

    public DbSet<UserRight> UserRights { get; set; }

    public DbSet<MaterialGroup> MaterialGroups { get; set; }

    public DbSet<Material> Materials { get; set; }

    public DbSet<FeedingPath> FeedingPaths { get; set; }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeItem> RecipeItems { get; set; }

    public DbSet<ProductionOrder> ProductionOrder { get; set; }

    public MesDbContext(DbContextOptions<MesDbContext> options) : base(options)
    {
    }

    // Optional: Advanced Configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
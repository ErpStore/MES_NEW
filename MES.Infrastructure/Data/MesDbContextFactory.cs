using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Data
{
    public class MesDbContextFactory : IDesignTimeDbContextFactory<MesDbContext>
    {
        public MesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MesDbContext>();

            // 1. Paste your Connection String here
            // (Ensure this matches what is in your DependencyInjection.cs)
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MES_Trikala_DB;Trusted_Connection=True;MultipleActiveResultSets=true";

            // 2. Configure SQL Server
            optionsBuilder.UseSqlServer(connectionString);

            // 3. Return the Context
            return new MesDbContext(optionsBuilder.Options);
        }
    }
}

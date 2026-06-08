using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevReview.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Use same connection string as appsettings.json; change if needed for your environment
            var connectionString = "Host=ep-dark-term-agk8ij4z.c-2.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_mvC2RPVAS7jy;SSL Mode=VerifyFull;Search Path=public";
            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("DevReview.Infrastructure"));
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

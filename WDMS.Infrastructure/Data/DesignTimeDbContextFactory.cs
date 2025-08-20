using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WDMS.Infrastructure.Data;

namespace WDMS.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Directly specify the connection string
            var connectionString = "Server=localhost;Database=WDM;Trusted_Connection=True;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RevenuQuebec.Infrastructure
{
    public class RevenuQuebecContextFactory : IDesignTimeDbContextFactory<RevenuQuebecContext>
    {
        public RevenuQuebecContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RevenuQuebecContext>();
            optionsBuilder.UseSqlServer(@"Server=.;Database=RevenuQuebec_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new RevenuQuebecContext(optionsBuilder.Options);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingSystem.Infrastructure.Data
{
    public class BankingDbContextFactory
        : IDesignTimeDbContextFactory<BankingDbContext>
    {
        public BankingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=BankingSystem;User Id=sa;Password=Banking@123!;TrustServerCertificate=True");

            return new BankingDbContext(optionsBuilder.Options);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=D:\\My portofilio\\dev-interactive-studio\\Backend\\data\\PortfolioDevDb.mdf;Database=PortfolioDevDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
        return new AppDbContext(optionsBuilder.Options);
    }
}

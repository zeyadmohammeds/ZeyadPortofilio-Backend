using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Services;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var useInMemory = configuration.GetValue<bool>("Database:UseInMemory");
        if (useInMemory)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("portfolio-dev"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? "Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=D:\\My portofilio\\dev-interactive-studio\\Backend\\data\\PortfolioDevDb.mdf;Database=PortfolioDevDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
        }
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IEmailService, ConsoleEmailService>();

        return services;
    }
}

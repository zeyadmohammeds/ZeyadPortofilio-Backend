using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }

        const string adminEmail = "zeyad.shosha@outlook.com";
        if (!await context.AdminUsers.AnyAsync(x => x.Email == adminEmail, cancellationToken))
        {
            context.AdminUsers.Add(new AdminUser
            {
                Email = adminEmail,
                PasswordHash = passwordHasher.Hash("admin1234"),
                Role = "Admin",
                IsActive = true
            });
        }

        if (!await context.Projects.AnyAsync(cancellationToken))
        {
            context.Projects.AddRange(
                new Project
                {
                    Name = "Dev Interactive Studio",
                    Tagline = "Interactive developer platform",
                    Description = "Enterprise-ready project for portfolio demonstration",
                    Type = Domain.Enums.ProjectType.Fullstack,
                    Stack = "ASP.NET Core,React,SQL Server",
                    Year = 2026,
                    RepoName = "dev-interactive-studio",
                    MetricsJson = "{\"requestsPerDay\":12000}"
                });
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

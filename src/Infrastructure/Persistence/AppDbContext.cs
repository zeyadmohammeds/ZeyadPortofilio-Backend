using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Common;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Education> Education => Set<Education>();
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.RepoName).HasMaxLength(200);
            e.Property(x => x.Tagline).HasMaxLength(300);
            e.HasIndex(x => x.Name);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Education>(e =>
        {
            e.Property(x => x.School).HasMaxLength(250);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<ContactSubmission>(e =>
        {
            e.Property(x => x.Email).HasMaxLength(200);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AdminUser>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.AdminUser).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.AdminUserId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

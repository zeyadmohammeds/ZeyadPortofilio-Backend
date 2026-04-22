using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Common;

namespace Portfolio.Infrastructure.Persistence;

public sealed class GenericRepository<T>(AppDbContext dbContext) : IRepository<T> where T : AuditableEntity
{
    public IQueryable<T> Query() => dbContext.Set<T>();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => dbContext.Set<T>().AddAsync(entity, cancellationToken).AsTask();

    public void Update(T entity) => dbContext.Set<T>().Update(entity);

    public void Remove(T entity)
    {
        entity.IsDeleted = true;
        dbContext.Set<T>().Update(entity);
    }
}

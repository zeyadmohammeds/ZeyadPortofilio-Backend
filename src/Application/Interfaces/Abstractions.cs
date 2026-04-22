namespace Portfolio.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

public interface IJwtTokenService
{
    string CreateAccessToken(Guid userId, string email, string role);
    string CreateRefreshToken();
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
}

public interface IFileStorageService
{
    Task<string> SaveProjectImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
}

public interface IEmailService
{
    Task SendContactReceivedAsync(string toEmail, string name, string message, CancellationToken cancellationToken);
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Auth;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;
public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, string Role);

public sealed class LoginCommandHandler(
    IRepository<AdminUser> users,
    IRepository<RefreshToken> refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await users.Query().FirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive, cancellationToken)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!hasher.Verify(user.PasswordHash, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var accessToken = jwtTokenService.CreateAccessToken(user.Id, user.Email, user.Role);
        var refreshTokenValue = jwtTokenService.CreateRefreshToken();
        var refreshToken = new RefreshToken
        {
            AdminUserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
        await refreshTokens.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshTokenValue, DateTime.UtcNow.AddMinutes(20), user.Role);
    }
}

public sealed class RefreshTokenCommandHandler(
    IRepository<RefreshToken> refreshTokens,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await refreshTokens.Query()
            .Include(x => x.AdminUser)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken && !x.Revoked, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (token.ExpiresAtUtc < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token expired.");
        }

        token.Revoked = true;
        var accessToken = jwtTokenService.CreateAccessToken(token.AdminUser.Id, token.AdminUser.Email, token.AdminUser.Role);
        var refreshTokenValue = jwtTokenService.CreateRefreshToken();
        await refreshTokens.AddAsync(new RefreshToken
        {
            AdminUserId = token.AdminUserId,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new LoginResponse(accessToken, refreshTokenValue, DateTime.UtcNow.AddMinutes(20), token.AdminUser.Role);
    }
}

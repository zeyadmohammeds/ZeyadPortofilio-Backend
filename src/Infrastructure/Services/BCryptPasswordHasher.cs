using BCrypt.Net;
using Portfolio.Application.Interfaces;

namespace Portfolio.Infrastructure.Services;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string hash, string password) => BCrypt.Net.BCrypt.Verify(password, hash);
}

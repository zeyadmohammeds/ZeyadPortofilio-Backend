using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public sealed class RefreshToken : AuditableEntity
{
    public Guid AdminUserId { get; set; }
    public AdminUser AdminUser { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool Revoked { get; set; }
}

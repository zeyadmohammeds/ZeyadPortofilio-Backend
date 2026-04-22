using System.Security.Claims;
using Portfolio.Application.Interfaces;

namespace Portfolio.API.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name), out var id) ? id : null;
    public string? Email => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
}

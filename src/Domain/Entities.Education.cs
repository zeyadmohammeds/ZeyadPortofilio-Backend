using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public sealed class Education : AuditableEntity
{
    public string School { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string? Focus { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Notes { get; set; }
}

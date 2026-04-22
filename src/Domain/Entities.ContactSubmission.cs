using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public sealed class ContactSubmission : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Processed { get; set; }
}

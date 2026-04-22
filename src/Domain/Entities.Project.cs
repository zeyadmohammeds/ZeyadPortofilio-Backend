using Portfolio.Domain.Common;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

public sealed class Project : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectType Type { get; set; }
    public string Stack { get; set; } = string.Empty;
    public string MetricsJson { get; set; } = "{}";
    public int Year { get; set; }
    public string? Url { get; set; }
    public string RepoName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

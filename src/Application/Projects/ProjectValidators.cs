using FluentValidation;

namespace Portfolio.Application.Projects;

public sealed class UpsertProjectCommandValidator : AbstractValidator<UpsertProjectCommand>
{
    public UpsertProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Tagline).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Type).NotEmpty().Must(v => new[] { "frontend", "backend", "fullstack", "mobile" }.Contains(v.ToLowerInvariant()));
        RuleFor(x => x.Stack).NotNull().Must(x => x.Length > 0);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
        RuleFor(x => x.RepoName).NotEmpty();
    }
}

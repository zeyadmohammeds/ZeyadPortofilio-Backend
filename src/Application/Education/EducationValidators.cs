using FluentValidation;

namespace Portfolio.Application.Education;

public sealed class UpsertEducationValidator : AbstractValidator<UpsertEducationCommand>
{
    public UpsertEducationValidator()
    {
        RuleFor(x => x.School).NotEmpty();
        RuleFor(x => x.Degree).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
    }
}

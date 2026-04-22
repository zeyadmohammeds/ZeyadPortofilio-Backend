using FluentValidation;

namespace Portfolio.Application.Contact;

public sealed class SubmitContactValidator : AbstractValidator<SubmitContactCommand>
{
    public SubmitContactValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Message).NotEmpty().MinimumLength(10).MaximumLength(4000);
    }
}

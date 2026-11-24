using FluentValidation;

namespace Transporters.Application.Transporters.CreateTransporter;

public sealed class CreateTransporterCommandValidator
    : AbstractValidator<CreateTransporterCommand>
{
    public CreateTransporterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ContactPerson)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .MaximumLength(200);
    }
}

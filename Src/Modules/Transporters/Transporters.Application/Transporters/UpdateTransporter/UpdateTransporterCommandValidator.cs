using FluentValidation;

namespace Transporters.Application.Transporters.UpdateTransporter;

public sealed class UpdateTransporterCommandValidator
    : AbstractValidator<UpdateTransporterCommand>
{
    public UpdateTransporterCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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

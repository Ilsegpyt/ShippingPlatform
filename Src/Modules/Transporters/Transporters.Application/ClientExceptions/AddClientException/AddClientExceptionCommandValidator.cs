using FluentValidation;

namespace Transporters.Application.ClientExceptions.AddClientException;

public sealed class AddClientExceptionCommandValidator
    : AbstractValidator<AddClientExceptionCommand>
{
    public AddClientExceptionCommandValidator()
    {
        RuleFor(x => x.TransporterId)
            .NotEmpty();

        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.PortCoverageId)
            .NotEmpty();

        RuleFor(x => x.Destination)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.EquipmentType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.AgreedPrice)
            .GreaterThan(0);

        RuleFor(x => x.ValidToUtc)
            .GreaterThan(x => x.ValidFromUtc);

        RuleFor(x => x.Reason)
            .MaximumLength(1000);
    }
}

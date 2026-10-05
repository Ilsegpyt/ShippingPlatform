using FluentValidation;

namespace Operations.Application.Operations.CreateOperation;

public sealed class CreateOperationValidator
    : AbstractValidator<CreateOperationCommand>
{
    public CreateOperationValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client is required.");

        RuleFor(x => x.ShippingLineId)
            .NotEmpty()
            .WithMessage("Shipping line is required.");

        RuleFor(x => x.OperationType)
            .IsInEnum()
            .WithMessage("Invalid operation type.");

        RuleFor(x => x.ShipmentNumber)
            .MaximumLength(120)
            .When(x => x.ShipmentNumber is not null);

        RuleFor(x => x.CertificateNumber)
            .MaximumLength(80)
            .When(x => x.CertificateNumber is not null);
    }
}
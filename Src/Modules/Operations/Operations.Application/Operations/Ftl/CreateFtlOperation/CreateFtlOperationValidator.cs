using FluentValidation;

namespace Operations.Application.Operations.Ftl.CreateFtlOperation;

public sealed class CreateFtlOperationValidator : AbstractValidator<CreateFtlOperationCommand>
{
    public CreateFtlOperationValidator()
    {
        RuleFor(x => x.Direction)
            .NotEmpty().WithMessage("Direction is required.")
            .Must(d => string.Equals(d, "Import", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(d, "Export", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Direction must be either Import or Export.");

        When(x => x.RowCount.HasValue, () =>
        {
            RuleFor(x => x.RowCount!.Value)
                .InclusiveBetween(1, 200)
                .WithMessage("Truck count must be between 1 and 200.");
        });

        When(x => x.ContainerCount.HasValue, () =>
        {
            RuleFor(x => x.ContainerCount!.Value)
                .InclusiveBetween(1, 200)
                .WithMessage("Shipment count must be between 1 and 200.");
        });
    }
}


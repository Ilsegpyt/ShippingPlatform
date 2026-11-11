using FluentValidation;

namespace Operations.Application.Operations.Ftl.UpdateFtlOperation;

public sealed class UpdateFtlOperationValidator : AbstractValidator<UpdateFtlOperationCommand>
{
    public UpdateFtlOperationValidator()
    {
        RuleFor(x => x.Shipments)
            .NotNull()
            .WithMessage("Shipments list cannot be null.");
    }
}


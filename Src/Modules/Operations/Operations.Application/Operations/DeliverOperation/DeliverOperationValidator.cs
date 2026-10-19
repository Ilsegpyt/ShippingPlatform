using FluentValidation;

namespace Operations.Application.Operations.DeliverOperation;

public sealed class DeliverOperationValidator
    : AbstractValidator<DeliverOperationCommand>
{
    public DeliverOperationValidator()
    {
        RuleFor(x => x.OperationId)
            .NotEmpty()
            .WithMessage("Operation is required.");
    }
}

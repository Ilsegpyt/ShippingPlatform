using FluentValidation;

namespace Operations.Application.Operations.ReopenOperation;

public sealed class ReopenOperationValidator
    : AbstractValidator<ReopenOperationCommand>
{
    public ReopenOperationValidator()
    {
        RuleFor(x => x.OperationId)
            .NotEmpty()
            .WithMessage("Operation is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("A reason is required to reopen an operation.")
            .MaximumLength(500)
            .WithMessage("Reopen reason cannot exceed 500 characters.");
    }
}

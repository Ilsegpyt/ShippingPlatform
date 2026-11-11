using FluentValidation;

namespace Operations.Application.Operations.Ftl.UnlockFtlOperation;

public sealed class UnlockFtlOperationValidator : AbstractValidator<UnlockFtlOperationCommand>
{
    public UnlockFtlOperationValidator()
    {
        RuleFor(x => x.Ref)
            .NotEmpty()
            .WithMessage("Operation reference is required.");
    }
}


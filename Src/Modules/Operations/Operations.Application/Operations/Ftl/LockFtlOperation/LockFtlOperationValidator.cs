using FluentValidation;

namespace Operations.Application.Operations.Ftl.LockFtlOperation;

public sealed class LockFtlOperationValidator : AbstractValidator<LockFtlOperationCommand>
{
    public LockFtlOperationValidator()
    {
        RuleFor(x => x.Ref)
            .NotEmpty()
            .WithMessage("Operation reference is required.");
    }
}


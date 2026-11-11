using FluentValidation;

namespace Operations.Application.Operations.Ftl.DeleteFtlOperation;

public sealed class DeleteFtlOperationValidator : AbstractValidator<DeleteFtlOperationCommand>
{
    public DeleteFtlOperationValidator()
    {
        RuleFor(x => x.Ref)
            .NotEmpty()
            .WithMessage("Operation reference is required.");
    }
}


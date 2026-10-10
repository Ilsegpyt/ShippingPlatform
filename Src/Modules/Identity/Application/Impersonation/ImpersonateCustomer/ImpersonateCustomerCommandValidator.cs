using FluentValidation;

namespace Identity.Application.Impersonation.ImpersonateCustomer;

public sealed class ImpersonateCustomerCommandValidator
    : AbstractValidator<ImpersonateCustomerCommand>
{
    public ImpersonateCustomerCommandValidator()
    {
        RuleFor(x => x.ImpersonatorUserId)
            .NotEmpty()
            .WithMessage("Impersonator user ID is required.");

        RuleFor(x => x.TargetCustomerUserId)
            .NotEmpty()
            .WithMessage("Target customer user ID is required.");

        RuleFor(x => x.TargetCustomerUserId)
            .NotEqual(x => x.ImpersonatorUserId)
            .WithMessage("You cannot impersonate yourself.");
    }
}

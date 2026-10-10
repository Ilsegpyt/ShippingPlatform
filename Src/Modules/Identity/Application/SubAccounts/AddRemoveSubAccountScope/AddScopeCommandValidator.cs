
using FluentValidation;
using Identity.Application.SubAccounts.AddRemoveSubAccountScope;

namespace Identity.Application.SubAccounts.UpdateSubAccountScope;

public sealed class AddScopeCommandValidator
    : AbstractValidator<AddScopeCommand>
{
    public AddScopeCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.SubAccountId)
            .NotEmpty()
            .WithMessage("Sub-account ID is required.");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid scope category.");

        RuleFor(x => x.Service)
            .IsInEnum()
            .WithMessage("Invalid scope service.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid shipment type.");
    }
}

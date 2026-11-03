using FluentValidation;

namespace Operations.Application.Operations.CreateShippingLine;

public sealed class CreateShippingLineValidator : AbstractValidator<CreateShippingLineCommand>
{
    public CreateShippingLineValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Shipping line name is required.").MaximumLength(160);
    }
}

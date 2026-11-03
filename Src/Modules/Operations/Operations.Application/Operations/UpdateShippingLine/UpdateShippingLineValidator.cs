using FluentValidation;

namespace Operations.Application.Operations.UpdateShippingLine;

public sealed class UpdateShippingLineValidator : AbstractValidator<UpdateShippingLineCommand>
{
    public UpdateShippingLineValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Shipping line name is required.").MaximumLength(160);
        RuleFor(x => x.Abbreviation).MaximumLength(50);
    }
}

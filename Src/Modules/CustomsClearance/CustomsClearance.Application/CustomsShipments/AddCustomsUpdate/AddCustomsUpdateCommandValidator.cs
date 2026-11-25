using FluentValidation;

namespace CustomsClearance.Application.CustomsShipments.AddCustomsUpdate;

public sealed class AddCustomsUpdateCommandValidator
    : AbstractValidator<AddCustomsUpdateCommand>
{
    public AddCustomsUpdateCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty();

        RuleFor(x => x.UpdateText)
            .NotEmpty()
            .MaximumLength(4000);
    }
}

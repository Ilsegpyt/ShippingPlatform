using FluentValidation;

namespace Shipments.Application.Shipments.UpdateShipment;

public sealed class UpdateShipmentCommandValidator
    : AbstractValidator<UpdateShipmentCommand>
{
    public UpdateShipmentCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty()
            .WithMessage("Shipment is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid shipment status.");
    }
}
using FluentValidation;

namespace Shipments.Application.Shipments.DeleteShipments;

public sealed class DeleteShipmentsCommandValidator
    : AbstractValidator<DeleteShipmentsCommand>
{
    public DeleteShipmentsCommandValidator()
    {
        RuleFor(x => x.ShipmentIds)
            .NotEmpty()
            .WithMessage("At least one shipment is required.");

        RuleForEach(x => x.ShipmentIds)
            .NotEmpty()
            .WithMessage("Shipment ID cannot be empty.");
    }
}
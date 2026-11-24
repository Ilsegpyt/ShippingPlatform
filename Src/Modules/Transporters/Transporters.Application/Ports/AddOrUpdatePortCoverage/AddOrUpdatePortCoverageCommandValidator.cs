using FluentValidation;

namespace Transporters.Application.Ports.AddOrUpdatePortCoverage;

public sealed class AddOrUpdatePortCoverageCommandValidator
    : AbstractValidator<AddOrUpdatePortCoverageCommand>
{
    public AddOrUpdatePortCoverageCommandValidator()
    {
        RuleFor(x => x.TransporterId)
            .NotEmpty();

        RuleFor(x => x.PortName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.FreeHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DelayRatePerHour)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.OfficialReceiptNotes)
            .MaximumLength(500);

        RuleFor(x => x.GeneralNotes)
            .MaximumLength(2000);

        RuleFor(x => x.LaneRates)
            .NotNull();

        RuleForEach(x => x.LaneRates).ChildRules(lane =>
        {
            lane.RuleFor(l => l.Destination)
                .NotEmpty()
                .MaximumLength(200);

            lane.RuleFor(l => l.EquipmentType)
                .NotEmpty()
                .MaximumLength(50);

            lane.RuleFor(l => l.Price)
                .GreaterThan(0);

            lane.RuleFor(l => l.Notes)
                .MaximumLength(1000);
        });
    }
}

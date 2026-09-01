using FluentValidation;

namespace Schedules.Application.Schedules.ImportSchedules;

public sealed class ImportScheduleRowValidator
    : AbstractValidator<ImportScheduleRow>
{
    public ImportScheduleRowValidator()
    {
        RuleFor(x => x.Vessel)
            .NotEmpty();

        RuleFor(x => x.Origin)
            .NotEmpty();

        RuleFor(x => x.DeparturePortCode)
            .NotEmpty();

        RuleFor(x => x.Destination)
            .NotEmpty();

        RuleFor(x => x.ArrivalPortCode)
            .NotEmpty();

        RuleFor(x => x.Carrier)
            .NotEmpty();

        RuleFor(x => x.CarrierCode)
            .NotEmpty();

        RuleFor(x => x.VoyageNumber)
            .NotEmpty();

        RuleFor(x => x.RateCurrency)
            .NotEmpty();

        RuleFor(x => x.RateAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.TransitTime)
            .GreaterThan(TimeSpan.Zero);

        RuleFor(x => x.Arrival)
            .GreaterThanOrEqualTo(x => x.DepartureDate);
    }
}
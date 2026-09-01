using FluentValidation;

namespace Schedules.Application.Schedules.CreateSchedule;

public sealed class CreateScheduleCommandValidator
    : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.DepartureDate)
            .NotEmpty();

        RuleFor(x => x.Vessel)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Origin)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DeparturePortCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.DepartureCountry)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Destination)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ArrivalPortCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.ArrivalCountry)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Carrier)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CarrierCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.VoyageNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Arrival)
            .GreaterThanOrEqualTo(x => x.DepartureDate)
            .WithMessage("Arrival date cannot be before departure date.");

        RuleFor(x => x.TransitTime)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .WithMessage("Transit time cannot be negative.");

        RuleFor(x => x.CutoffDate)
            .LessThan(x => x.DepartureDate)
            .WithMessage("Cut-off date must be before departure date.");

        RuleFor(x => x.RateCurrency)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.RateAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Rate amount cannot be negative.");

        RuleFor(x => x.RateRemarks)
            .MaximumLength(500);

        RuleFor(x => x.ValidityDate)
            .NotEmpty();

        RuleFor(x => x.FreeTimeAtPOD)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Free time at POD cannot be negative.");

        RuleFor(x => x.FreeTimeAtPOL)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Free time at POL cannot be negative.");

        RuleFor(x => x.TransshipmentData)
            .MaximumLength(1000);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}

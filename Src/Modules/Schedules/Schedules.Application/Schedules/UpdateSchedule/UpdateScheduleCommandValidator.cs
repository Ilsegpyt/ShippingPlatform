using FluentValidation;

namespace Schedules.Application.Schedules.UpdateSchedule;

public sealed class UpdateScheduleCommandValidator
    : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Schedule ID is required.");

        RuleFor(x => x.RouteId)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Route ID cannot be empty.")
            .MaximumLength(100)
            .When(x => x.RouteId is not null);

        RuleFor(x => x.Mode)
            .Must(value => !value.HasValue || Enum.IsDefined(value.Value))
            .WithMessage("Invalid schedule mode.");

        RuleFor(x => x.DepartureDate)
            .NotNull()
            .When(x => x.DepartureDate.HasValue == false)
            .WithMessage("Departure date is required.");

        RuleFor(x => x.Vessel)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Vessel cannot be empty.")
            .MaximumLength(200)
            .When(x => x.Vessel is not null);

        RuleFor(x => x.Origin)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Origin cannot be empty.")
            .MaximumLength(100)
            .When(x => x.Origin is not null);

        RuleFor(x => x.DeparturePortCode)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Departure port code cannot be empty.")
            .MaximumLength(20)
            .When(x => x.DeparturePortCode is not null);

        RuleFor(x => x.DepartureCountry)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Departure country cannot be empty.")
            .MaximumLength(100)
            .When(x => x.DepartureCountry is not null);

        RuleFor(x => x.Destination)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Destination cannot be empty.")
            .MaximumLength(100)
            .When(x => x.Destination is not null);

        RuleFor(x => x.ArrivalPortCode)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Arrival port code cannot be empty.")
            .MaximumLength(20)
            .When(x => x.ArrivalPortCode is not null);

        RuleFor(x => x.ArrivalCountry)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Arrival country cannot be empty.")
            .MaximumLength(100)
            .When(x => x.ArrivalCountry is not null);

        RuleFor(x => x.Carrier)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Carrier cannot be empty.")
            .MaximumLength(200)
            .When(x => x.Carrier is not null);

        RuleFor(x => x.CarrierCode)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Carrier code cannot be empty.")
            .MaximumLength(50)
            .When(x => x.CarrierCode is not null);

        RuleFor(x => x.VoyageNumber)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Voyage number cannot be empty.")
            .MaximumLength(100)
            .When(x => x.VoyageNumber is not null);

        RuleFor(x => x.RateCurrency)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Rate currency cannot be empty.")
            .MaximumLength(10)
            .When(x => x.RateCurrency is not null);

        RuleFor(x => x.ContainerSize)
            .Must(value => !value.HasValue || Enum.IsDefined(value.Value))
            .WithMessage("Invalid container size.");

        RuleFor(x => x.TransitTime)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .When(x => x.TransitTime.HasValue)
            .WithMessage("Transit time cannot be negative.");

        RuleFor(x => x.RateAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.RateAmount.HasValue)
            .WithMessage("Rate amount cannot be negative.");

        RuleFor(x => x.FreeTimeAtPOD)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FreeTimeAtPOD.HasValue)
            .WithMessage("Free time at POD cannot be negative.");

        RuleFor(x => x.FreeTimeAtPOL)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FreeTimeAtPOL.HasValue)
            .WithMessage("Free time at POL cannot be negative.");

        RuleFor(x => x.RateRemarks)
            .MaximumLength(1000)
            .When(x => x.RateRemarks is not null);

        RuleFor(x => x.TransshipmentData)
            .MaximumLength(2000)
            .When(x => x.TransshipmentData is not null);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => x.Notes is not null);
    }
}
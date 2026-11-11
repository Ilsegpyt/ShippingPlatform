using FluentValidation;

namespace Operations.Application.Operations.Ftl.ImportFtlTrips;

public sealed class ImportFtlTripsValidator : AbstractValidator<ImportFtlTripsCommand>
{
    public ImportFtlTripsValidator()
    {
        RuleFor(x => x.Trips)
            .NotNull().WithMessage("Trips list cannot be null.")
            .Must(t => t != null && t.Count > 0).WithMessage("At least one trip row is required for import.");

        RuleForEach(x => x.Trips).ChildRules(trip =>
        {
            trip.RuleFor(t => t.Direction)
                .NotEmpty().WithMessage("Trip direction is required.")
                .Must(d => string.Equals(d, "Import", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(d, "Export", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Trip direction must be Import or Export.");

            trip.RuleFor(t => t.Fields)
                .NotNull().WithMessage("Trip fields dictionary cannot be null.");
        });
    }
}

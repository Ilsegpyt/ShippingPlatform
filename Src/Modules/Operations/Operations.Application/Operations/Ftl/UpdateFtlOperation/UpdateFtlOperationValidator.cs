using FluentValidation;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.UpdateFtlOperation;

public sealed class UpdateFtlOperationValidator : AbstractValidator<UpdateFtlOperationCommand>
{
    public UpdateFtlOperationValidator()
    {
        RuleFor(x => x.Shipments)
            .NotNull()
            .WithMessage("Shipments list cannot be null.")
            .NotEmpty()
            .WithMessage("At least one shipment is required.");

        RuleForEach(x => x.Shipments)
            .SetValidator(new FtlOperationDtoValidator());
    }
}

public sealed class FtlOperationDtoValidator : AbstractValidator<FtlOperationDto>
{
    public FtlOperationDtoValidator()
    {
        RuleFor(x => x.Ref)
            .NotEmpty()
            .WithMessage("Operation Reference is required.")
            .MaximumLength(50)
            .WithMessage("Operation Reference cannot exceed 50 characters.");

        RuleFor(x => x.Trips)
            .NotNull()
            .WithMessage("Trips list cannot be null.")
            .NotEmpty()
            .WithMessage("At least one trip row is required.");

        RuleForEach(x => x.Trips)
            .SetValidator(new FtlTripDtoValidator());
    }
}

public sealed class FtlTripDtoValidator : AbstractValidator<FtlTripDto>
{
    public FtlTripDtoValidator()
    {
        RuleFor(x => x.Fields)
            .NotNull()
            .WithMessage("Trip fields cannot be null.");

        RuleFor(x => x.Fields)
            .Must(fields =>
            {
                if (fields == null) return true;
                if (fields.TryGetValue("cntr", out var cntr) && !string.IsNullOrWhiteSpace(cntr))
                {
                    return cntr.Trim().Length <= 25;
                }
                return true;
            })
            .WithMessage("Truck plate / container number cannot exceed 25 characters.");

        RuleFor(x => x.Fields)
            .Must(fields =>
            {
                if (fields == null) return true;
                if (fields.TryGetValue("dest", out var dest) && !string.IsNullOrWhiteSpace(dest))
                {
                    return dest.Trim().Length <= 200;
                }
                return true;
            })
            .WithMessage("Destination cannot exceed 200 characters.");

        RuleFor(x => x.Fields)
            .Must(fields =>
            {
                if (fields == null) return true;
                if (fields.TryGetValue("driver", out var driver) && !string.IsNullOrWhiteSpace(driver))
                {
                    return driver.Trim().Length <= 120;
                }
                return true;
            })
            .WithMessage("Driver name cannot exceed 120 characters.");

        RuleFor(x => x.Fields)
            .Must(fields =>
            {
                if (fields == null) return true;
                if (fields.TryGetValue("mob", out var mob) && !string.IsNullOrWhiteSpace(mob))
                {
                    return mob.Trim().Length <= 30;
                }
                return true;
            })
            .WithMessage("Driver mobile cannot exceed 30 characters.");

        RuleFor(x => x.Fields)
            .Must(fields =>
            {
                if (fields == null) return true;
                if (fields.TryGetValue("customsArea", out var customs) && !string.IsNullOrWhiteSpace(customs))
                {
                    return customs.Trim().Length <= 150;
                }
                if (fields.TryGetValue("customs", out var custShort) && !string.IsNullOrWhiteSpace(custShort))
                {
                    return custShort.Trim().Length <= 150;
                }
                return true;
            })
            .WithMessage("Customs area cannot exceed 150 characters.");

        RuleFor(x => x.Fields)
            .Must(fields =>
            {
                if (fields == null) return true;
                if (fields.TryGetValue("cert", out var cert) && !string.IsNullOrWhiteSpace(cert))
                {
                    return cert.Trim().Length <= 100;
                }
                return true;
            })
            .WithMessage("Certificate number cannot exceed 100 characters.");

        RuleFor(x => x.LoadedPackages)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LoadedPackages.HasValue)
            .WithMessage("Loaded packages cannot be negative.");
    }
}


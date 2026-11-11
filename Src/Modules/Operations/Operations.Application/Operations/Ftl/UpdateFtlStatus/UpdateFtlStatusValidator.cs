using FluentValidation;

namespace Operations.Application.Operations.Ftl.UpdateFtlStatus;

public sealed class UpdateFtlStatusValidator : AbstractValidator<UpdateFtlStatusCommand>
{
    public UpdateFtlStatusValidator()
    {
        RuleFor(x => x.Ref)
            .NotEmpty()
            .WithMessage("Operation reference is required.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.")
            .Must(s => string.Equals(s, "Open", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(s, "Delivered", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Status must be either Open or Delivered.");
    }
}


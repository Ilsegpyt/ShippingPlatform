using FluentValidation;

namespace Operations.Application.Operations.Ftl.AddFtlTruckUpdate;

public sealed class AddFtlTruckUpdateValidator : AbstractValidator<AddFtlTruckUpdateCommand>
{
    public AddFtlTruckUpdateValidator()
    {
        RuleFor(x => x.Ref)
            .NotEmpty()
            .WithMessage("Operation reference is required.");

        RuleFor(x => x.Note)
            .NotEmpty()
            .WithMessage("Update note is required.")
            .MaximumLength(500)
            .WithMessage("Update note must not exceed 500 characters.");
    }
}


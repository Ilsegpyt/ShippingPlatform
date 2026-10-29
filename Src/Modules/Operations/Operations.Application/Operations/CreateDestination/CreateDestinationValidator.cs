using FluentValidation;

namespace Operations.Application.Operations.CreateDestination;

public sealed class CreateDestinationValidator
    : AbstractValidator<CreateDestinationCommand>
{
    public CreateDestinationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Destination name is required.")
            .MaximumLength(200)
            .WithMessage("Destination name must not exceed 200 characters.");
    }
}

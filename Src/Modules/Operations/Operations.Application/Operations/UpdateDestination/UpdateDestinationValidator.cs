using FluentValidation;

namespace Operations.Application.Operations.UpdateDestination;

public sealed class UpdateDestinationValidator : AbstractValidator<UpdateDestinationCommand>
{
    public UpdateDestinationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Destination name is required.").MaximumLength(200);
    }
}

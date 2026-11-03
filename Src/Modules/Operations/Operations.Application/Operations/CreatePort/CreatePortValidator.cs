using FluentValidation;

namespace Operations.Application.Operations.CreatePort;

public sealed class CreatePortValidator : AbstractValidator<CreatePortCommand>
{
    public CreatePortValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Port name is required.").MaximumLength(160);
        RuleFor(x => x.Code).MaximumLength(20);
    }
}

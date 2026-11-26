using FluentValidation;

namespace Operations.Application.Operations.CreatePortTerminal;

public sealed class CreatePortTerminalValidator : AbstractValidator<CreatePortTerminalCommand>
{
    public CreatePortTerminalValidator()
    {
        RuleFor(x => x.PortId)
            .NotEmpty()
            .WithMessage("Port is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Terminal name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .MaximumLength(40);

        RuleFor(x => x.ArabicName)
            .MaximumLength(200);
    }
}

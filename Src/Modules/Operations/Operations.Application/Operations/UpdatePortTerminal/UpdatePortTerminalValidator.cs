using FluentValidation;

namespace Operations.Application.Operations.UpdatePortTerminal;

public sealed class UpdatePortTerminalValidator : AbstractValidator<UpdatePortTerminalCommand>
{
    public UpdatePortTerminalValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

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

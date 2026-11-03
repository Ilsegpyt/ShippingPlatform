using FluentValidation;

namespace Operations.Application.Operations.CreateTransporter;

public sealed class CreateTransporterValidator : AbstractValidator<CreateTransporterCommand>
{
    public CreateTransporterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Transporter name is required.").MaximumLength(200);
    }
}

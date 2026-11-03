using FluentValidation;

namespace Operations.Application.Operations.UpdateTransporter;

public sealed class UpdateTransporterValidator : AbstractValidator<UpdateTransporterCommand>
{
    public UpdateTransporterValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Transporter name is required.").MaximumLength(200);
    }
}

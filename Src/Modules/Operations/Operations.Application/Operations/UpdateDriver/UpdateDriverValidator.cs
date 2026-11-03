using FluentValidation;

namespace Operations.Application.Operations.UpdateDriver;

public sealed class UpdateDriverValidator : AbstractValidator<UpdateDriverCommand>
{
    public UpdateDriverValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DriverNumber).NotEmpty().WithMessage("Driver number is required.").MaximumLength(80);
        RuleFor(x => x.Name).NotEmpty().WithMessage("Driver name is required.").MaximumLength(200);
        RuleFor(x => x.MobileNumber).MaximumLength(25);
    }
}

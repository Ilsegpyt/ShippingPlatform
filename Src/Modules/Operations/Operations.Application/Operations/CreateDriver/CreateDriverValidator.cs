using FluentValidation;

namespace Operations.Application.Operations.CreateDriver;

public sealed class CreateDriverValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverValidator()
    {
        RuleFor(x => x.DriverNumber).NotEmpty().WithMessage("Driver number is required.").MaximumLength(80);
        RuleFor(x => x.Name).NotEmpty().WithMessage("Driver name is required.").MaximumLength(200);
        RuleFor(x => x.MobileNumber).MaximumLength(25);
    }
}

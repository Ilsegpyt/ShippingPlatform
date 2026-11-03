using FluentValidation;

namespace Operations.Application.Operations.CreateVehicle;

public sealed class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.CarNumber).NotEmpty().WithMessage("Car number is required.").MaximumLength(80);
    }
}

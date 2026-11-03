using FluentValidation;

namespace Operations.Application.Operations.UpdateVehicle;

public sealed class UpdateVehicleValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CarNumber).NotEmpty().WithMessage("Car number is required.").MaximumLength(80);
    }
}

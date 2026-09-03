using FluentValidation;

namespace Shipments.Application.Shipments.CreateShipment;

public sealed class CreateShipmentCommandValidator
    : AbstractValidator<CreateShipmentCommand>
{
    private const long MaxFileSize = 10 * 1024 * 1024;

    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.DeclarationFiles)
            .NotEmpty()
            .WithMessage("At least one declaration file is required.");

        RuleForEach(x => x.DeclarationFiles)
            .Must(file => file.Content.Length <= MaxFileSize)
            .WithMessage("Each declaration file must not exceed 10 MB.");
    }
}
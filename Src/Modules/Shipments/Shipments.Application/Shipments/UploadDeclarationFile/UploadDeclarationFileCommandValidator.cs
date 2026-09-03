using FluentValidation;

namespace Shipments.Application.Shipments.UploadDeclarationFile;

public sealed class UploadDeclarationFileCommandValidator
    : AbstractValidator<UploadDeclarationFileCommand>
{
    private const long MaxFileSize = 10 * 1024 * 1024;

    public UploadDeclarationFileCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty()
            .WithMessage("Shipment is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer is required.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File is required.");

        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("File is required.");

        RuleFor(x => x.Content)
            .Must(file => file != null && file.Length <= MaxFileSize)
            .WithMessage("Declaration file must not exceed 10 MB.");
    }
}
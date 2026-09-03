using FluentValidation;

namespace Shipments.Application.Shipments.DeleteDeclarationFiles;

public sealed class DeleteDeclarationFilesCommandValidator
    : AbstractValidator<DeleteDeclarationFilesCommand>
{
    public DeleteDeclarationFilesCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty()
            .WithMessage("Shipment is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer is required.");

        RuleFor(x => x.DeclarationFileIds)
            .NotEmpty()
            .WithMessage("At least one declaration file is required.");

        RuleForEach(x => x.DeclarationFileIds)
            .NotEmpty()
            .WithMessage("Declaration file ID cannot be empty.");
    }
}
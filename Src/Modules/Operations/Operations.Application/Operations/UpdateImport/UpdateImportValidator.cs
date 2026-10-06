using FluentValidation;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateImport;

public sealed class UpdateImportValidator
    : AbstractValidator<UpdateImportCommand>
{
    public UpdateImportValidator()
    {
        RuleFor(x => x.OperationId)
            .NotEmpty()
            .WithMessage("Operation is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid operation status.");

        RuleFor(x => x.Containers)
            .NotNull()
            .WithMessage("Containers are required.");

        RuleForEach(x => x.Containers)
            .SetValidator(new UpdateImportContainerItemValidator());
    }
}

public sealed class UpdateImportContainerItemValidator
    : AbstractValidator<UpdateImportContainerItem>
{
    public UpdateImportContainerItemValidator()
    {
        RuleFor(x => x.ContainerId)
            .NotEmpty()
            .WithMessage("Container is required.");

        RuleFor(x => x.ContainerType)
            .IsInEnum()
            .WithMessage("Invalid container type.");

        RuleFor(x => x.ContainerNumber)
            .MaximumLength(20)
            .When(x => x.ContainerNumber is not null)
            .WithMessage("Container number cannot exceed 20 characters.");

        RuleFor(x => x.MissingDocs)
            .MaximumLength(500)
            .When(x => x.MissingDocs is not null)
            .WithMessage("Missing documents cannot exceed 500 characters.");
    }
}
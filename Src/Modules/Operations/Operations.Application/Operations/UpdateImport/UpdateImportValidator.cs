
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

        RuleFor(x => x.TerminalId)
            .Must(id => id != Guid.Empty)
            .When(x => x.TerminalId.HasValue)
            .WithMessage("Invalid terminal ID.");

        RuleFor(x => x)
            .Must(x => !(x.TerminalId.HasValue && x.ClearTerminal))
            .WithMessage("Cannot specify TerminalId and ClearTerminal at the same time.");

        RuleFor(x => x.Containers)
            .NotNull()
            .WithMessage("Containers are required.");

        RuleFor(x => x.Containers)
            .Must(items =>
                items is not null &&
                items.Select(x => x.ContainerId).Distinct().Count() == items.Count)
            .WithMessage("A container can only appear once in the request.");

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
            .When(x => x.ContainerType.HasValue)
            .WithMessage("Invalid container type.");

        RuleFor(x => x.ContainerNumber)
            .MaximumLength(20)
            .When(x => x.ContainerNumber is not null)
            .WithMessage("Container number cannot exceed 20 characters.");

        RuleFor(x => x.MissingDocs)
            .MaximumLength(500)
            .When(x => x.MissingDocs is not null)
            .WithMessage("Missing documents cannot exceed 500 characters.");

        RuleFor(x => x.Destination)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .When(x => x.Destination is not null)
            .WithMessage("Destination cannot be empty.");

        RuleFor(x => x.Destination)
            .MaximumLength(200)
            .When(x => x.Destination is not null)
            .WithMessage("Destination cannot exceed 200 characters.");
    }
}


using FluentValidation;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateExport;

public sealed class UpdateExportValidator : AbstractValidator<UpdateExportCommand>
{
    public UpdateExportValidator()
    {
        RuleFor(x => x.OperationId)
            .NotEmpty()
            .WithMessage("Operation is required.");

        RuleFor(x => x.Containers)
            .NotNull()
            .WithMessage("Containers are required.");

        RuleFor(x => x.Containers)
            .Must(items =>
                items is not null &&
                items.Select(x => x.ContainerId).Distinct().Count() == items.Count)
            .WithMessage("A container can only appear once in the request.");

        RuleForEach(x => x.Containers)
            .SetValidator(new UpdateExportContainerItemValidator());
    }
}

public sealed class UpdateExportContainerItemValidator
    : AbstractValidator<UpdateExportContainerItem>
{
    public UpdateExportContainerItemValidator()
    {
        RuleFor(x => x.ContainerId)
            .NotEmpty()
            .WithMessage("Container is required.");

        RuleFor(x => x.ContainerType)
            .NotNull()
            .WithMessage("Container type is required.")
            .IsInEnum()
            .WithMessage("Invalid container type.");

        RuleFor(x => x.ContainerNumber)
            .MaximumLength(11)
            .When(x => !string.IsNullOrWhiteSpace(x.ContainerNumber))
            .WithMessage("Container number cannot exceed 11 characters.");

        RuleFor(x => x.RequiredLoadingDate)
            .NotNull()
            .WithMessage("Required loading date is required.");

        RuleFor(x => x.RequiredLoadingTime)
            .NotNull()
            .WithMessage("Required loading time is required.");

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

using FluentValidation;

namespace CustomsClearance.Application.CustomsShipments.UpdateCustomsShipment;

public sealed class UpdateCustomsShipmentCommandValidator
    : AbstractValidator<UpdateCustomsShipmentCommand>
{
    public UpdateCustomsShipmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ShipmentNumber)
            .MaximumLength(120);

        RuleFor(x => x.CertificateNumber)
            .MaximumLength(120);

        RuleFor(x => x)
            .Must(x =>
                (x.OceanImportDetails is not null ? 1 : 0) +
                (x.OceanExportDetails is not null ? 1 : 0) +
                (x.DomesticImportDetails is not null ? 1 : 0) <= 1)
            .WithMessage("Only one details section can be sent.");

        // ---------- Ocean Import ----------
        When(x => x.OceanImportDetails is not null, () =>
        {
            RuleFor(x => x.OceanImportDetails!.Hbl)
                .MaximumLength(120);

            RuleFor(x => x.OceanImportDetails!.CustomsDeclarations)
                .MaximumLength(500);

            RuleFor(x => x.OceanImportDetails!.NumberOfContainers)
                .InclusiveBetween(1, 10000)
                .When(x => x.OceanImportDetails!.NumberOfContainers is not null);

            RuleFor(x => x.OceanImportDetails!.ContainerType)
                .Must(CustomsContainerTypes.IsValid)
                .When(x => !string.IsNullOrWhiteSpace(x.OceanImportDetails!.ContainerType))
                .WithMessage("Container type must be 20, 40, 40 HC or 45 HC.");

            RuleForEach(x => x.OceanImportDetails!.Containers)
                .Must(CustomsContainerTypes.IsValid)
                .WithMessage("Container type must be 20, 40, 40 HC or 45 HC.");

            RuleFor(x => x.OceanImportDetails!)
                .Must(d => d.NumberOfContainers == d.Containers!.Count)
                .When(x => x.OceanImportDetails!.Containers is { Count: > 0 })
                .WithMessage("One container type is needed for every container.");

            RuleFor(x => x.OceanImportDetails!.ShippingLine)
                .MaximumLength(200);

            RuleFor(x => x.OceanImportDetails!.Commodity)
                .MaximumLength(500);

            RuleFor(x => x.OceanImportDetails!.Pod)
                .MaximumLength(200);

            RuleFor(x => x.OceanImportDetails!.Terminal)
                .MaximumLength(200);

            RuleFor(x => x.OceanImportDetails!.EtaDate)
                .NotEmpty();

            RuleFor(x => x.OceanImportDetails!.FreeTime)
                .GreaterThanOrEqualTo(0)
                .When(x => x.OceanImportDetails!.FreeTime is not null);

            RuleFor(x => x.OceanImportDetails!.MissingDocs)
                .MaximumLength(2000);

            RuleFor(x => x.OceanImportDetails!.CurrentStage)
                .MaximumLength(200);
        });

        // ---------- Ocean Export ----------
        When(x => x.OceanExportDetails is not null, () =>
        {
            RuleFor(x => x.OceanExportDetails!.CustomsArea)
                .MaximumLength(200);

            RuleFor(x => x.OceanExportDetails!.ContainerType)
                .Must(CustomsContainerTypes.IsValid)
                .When(x => !string.IsNullOrWhiteSpace(x.OceanExportDetails!.ContainerType))
                .WithMessage("Container type must be 20, 40, 40 HC or 45 HC.");

            RuleForEach(x => x.OceanExportDetails!.Containers)
                .Must(CustomsContainerTypes.IsValid)
                .WithMessage("Container type must be 20, 40, 40 HC or 45 HC.");
        });

        // ---------- Domestic Import ----------
        When(x => x.DomesticImportDetails is not null, () =>
        {
            RuleFor(x => x.DomesticImportDetails!.MissingDocs)
                .MaximumLength(2000);
        });
    }
}

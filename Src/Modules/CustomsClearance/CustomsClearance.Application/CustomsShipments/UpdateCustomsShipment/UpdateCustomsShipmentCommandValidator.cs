using CustomsClearance.Domain.Enums;
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
                .WithMessage("Container type is not a supported FCL container type.");

            RuleForEach(x => x.OceanImportDetails!.Containers)
                .Must(CustomsContainerTypes.IsValid)
                .WithMessage("Container type is not a supported FCL container type.");

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

            // Alert 2: the user enters DateOfFreeTimeEnd by hand; it must equal AtaDate + (FreeTime - 1) days.
            RuleFor(x => x.OceanImportDetails!.DateOfFreeTimeEnd)
                .Custom((end, ctx) =>
                {
                    var d = ctx.InstanceToValidate.OceanImportDetails!;
                    if (end is null)
                        return;

                    if (d.AtaDate is null)
                        ctx.AddFailure("Enter the ATA date before the date of free time end.");
                    else if (d.FreeTime is null or < 1)
                        ctx.AddFailure("Free time must be at least 1 day before entering the date of free time end.");
                    else
                    {
                        var expected = d.AtaDate.Value.AddDays(d.FreeTime.Value - 1);
                        if (end.Value != expected)
                            ctx.AddFailure(
                                $"Date of free time end must be {expected:yyyy-MM-dd} (ATA {d.AtaDate.Value:yyyy-MM-dd} + {d.FreeTime.Value} free days).");
                    }
                });

            // Alert 3: D/O date needs an ATA date and cannot be before it.
            RuleFor(x => x.OceanImportDetails!.DoDate)
                .Custom((doDate, ctx) =>
                {
                    var d = ctx.InstanceToValidate.OceanImportDetails!;
                    if (doDate is null)
                        return;

                    if (d.AtaDate is null)
                        ctx.AddFailure("Enter the ATA date before the D/O date.");
                    else if (doDate.Value < d.AtaDate.Value)
                        ctx.AddFailure(
                            $"D/O date cannot be before the ATA date ({d.AtaDate.Value:yyyy-MM-dd}).");
                });

            RuleFor(x => x.OceanImportDetails!.MissingDocs)
                .MaximumLength(2000);

            RuleFor(x => x.OceanImportDetails!.Track)
                .Must(track => OceanImportTrack.IsValid(track!.Trim()))
                .When(x => !string.IsNullOrWhiteSpace(x.OceanImportDetails!.Track))
                .WithMessage("Track must be Blue, Yellow, Green, Red or GreenImports.");

            RuleFor(x => x.OceanImportDetails!.InspectionDate)
                .Null()
                .When(x => !OceanImportTrack.AllowsInspection(x.OceanImportDetails!.Track?.Trim()))
                .WithMessage("Inspection date is allowed only when the track is Red or Green imports. Clear it or change the track.");

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
                .WithMessage("Container type is not a supported FCL container type.");

            RuleForEach(x => x.OceanExportDetails!.Containers)
                .Must(CustomsContainerTypes.IsValid)
                .WithMessage("Container type is not a supported FCL container type.");
        });

        // ---------- Domestic Import ----------
        When(x => x.DomesticImportDetails is not null, () =>
        {
            RuleFor(x => x.DomesticImportDetails!.MissingDocs)
                .MaximumLength(2000);
        });
    }
}

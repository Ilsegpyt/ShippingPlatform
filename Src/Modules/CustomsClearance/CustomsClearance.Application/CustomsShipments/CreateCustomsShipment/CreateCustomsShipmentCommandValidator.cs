using CustomsClearance.Domain.Enums;
using FluentValidation;

namespace CustomsClearance.Application.CustomsShipments.CreateCustomsShipment;

public sealed class CreateCustomsShipmentCommandValidator
    : AbstractValidator<CreateCustomsShipmentCommand>
{
    public CreateCustomsShipmentCommandValidator()
    {
        RuleFor(x => x.ReportType)
            .IsInEnum();

        RuleFor(x => x.ClientId)
            .NotEmpty();

        // ---------- Ocean Import ----------
        When(x => x.ReportType == ReportType.OceanImport, () =>
        {
            RuleFor(x => x.OceanImportDetails)
                .NotNull();

            RuleFor(x => x.OceanExportDetails)
                .Null();

            RuleFor(x => x.DomesticImportDetails)
                .Null();

            When(x => x.OceanImportDetails is not null, () =>
            {
                RuleFor(x => x.OceanImportDetails!.ClearanceType)
                    .IsInEnum();

                RuleFor(x => x.OceanImportDetails!.AcidNumber)
                    .NotEmpty()
                    .Matches(@"^\d{19}$")
                    .WithMessage("ACID must contain exactly 19 digits.");

                RuleFor(x => x.OceanImportDetails!.Mbl)
                    .NotEmpty()
                    .MaximumLength(120);

                RuleFor(x => x.OceanImportDetails!.EtaDate)
                    .NotEmpty();
            });
        });

        // ---------- Ocean Export ----------
        When(x => x.ReportType == ReportType.OceanExport, () =>
        {
            RuleFor(x => x.OceanExportDetails)
                .NotNull();

            RuleFor(x => x.OceanImportDetails)
                .Null();

            RuleFor(x => x.DomesticImportDetails)
                .Null();

            When(x => x.OceanExportDetails is not null, () =>
            {
                RuleFor(x => x.OceanExportDetails!.ClearanceType)
                    .IsInEnum();

                RuleFor(x => x.OceanExportDetails!.Pol)
                    .NotEmpty()
                    .MaximumLength(200);

                RuleFor(x => x.OceanExportDetails!.Pod)
                    .NotEmpty()
                    .MaximumLength(200);

                RuleFor(x => x.OceanExportDetails!.BookingNumber)
                    .NotEmpty()
                    .MaximumLength(120);

                RuleFor(x => x.OceanExportDetails!.CommercialInvoiceNumber)
                    .NotEmpty()
                    .MaximumLength(120);

                RuleFor(x => x.OceanExportDetails!.NumberOfContainers)
                    .InclusiveBetween(1, 10000);
            });
        });

        // ---------- Domestic Import ----------
        When(x => x.ReportType == ReportType.Domestic, () =>
        {
            RuleFor(x => x.DomesticImportDetails)
                .NotNull();

            RuleFor(x => x.OceanImportDetails)
                .Null();

            RuleFor(x => x.OceanExportDetails)
                .Null();

            When(x => x.DomesticImportDetails is not null, () =>
            {
                RuleFor(x => x.DomesticImportDetails!.CommercialInvoiceNumber)
                    .NotEmpty()
                    .MaximumLength(120);

                RuleFor(x => x.DomesticImportDetails!.NumberOfPieces)
                    .GreaterThanOrEqualTo(1);
            });
        });
    }
}

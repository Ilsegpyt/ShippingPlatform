using FluentValidation;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.CreateOperation;

public sealed class CreateOperationValidator
    : AbstractValidator<CreateOperationCommand>
{
    public CreateOperationValidator()
    {
        // Common
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client is required.");

        RuleFor(x => x.ShippingLineId)
            .NotEmpty()
            .WithMessage("Shipping line is required.");

        RuleFor(x => x.OperationType)
            .IsInEnum()
            .WithMessage("Invalid operation type.");

        // =========================
        // Import
        // =========================

        When(x => x.OperationType == OperationType.Import, () =>
        {
            RuleFor(x => x.MBLNumber)
                .NotEmpty()
                .WithMessage("M/BL Number is required.")
                .MaximumLength(120);

            RuleFor(x => x.Destination)
                .NotEmpty()
                .WithMessage("Destination is required.")
                .MaximumLength(200);

            RuleFor(x => x.PODId)
                .NotEmpty()
                .WithMessage("POD is required.");

            RuleFor(x => x.FreeTimeTill)
                .NotNull()
                .WithMessage("Free Time Till is required.");

            RuleFor(x => x.RequiredOffloadingDate)
                .NotNull()
                .WithMessage("Required Offloading Date is required.");

            RuleFor(x => x.RequiredOffloadingTime)
                .NotNull()
                .WithMessage("Required Offloading Time is required.");

            // Export fields are not allowed for Import
            RuleFor(x => x.ClearanceType)
                .Null()
                .WithMessage("Clearance Type is not allowed for Import.");

            RuleFor(x => x.BookingNumber)
                .Null()
                .WithMessage("Booking Number is not allowed for Import.");

            RuleFor(x => x.POLId)
                .Null()
                .WithMessage("POL is not allowed for Import.");

            RuleFor(x => x.POWId)
                .Null()
                .WithMessage("POW is not allowed for Import.");

            RuleFor(x => x.CutOffDate)
                .Null()
                .WithMessage("Cut Off Date is not allowed for Import.");
        });

        // =========================
        // Export
        // =========================

        When(x => x.OperationType == OperationType.Export, () =>
        {
            RuleFor(x => x.ClearanceType)
                .NotNull()
                .WithMessage("Clearance Type is required.");

            RuleFor(x => x.BookingNumber)
                .NotEmpty()
                .WithMessage("Booking Number is required.")
                .MaximumLength(120);

            RuleFor(x => x.POLId)
                .NotEmpty()
                .WithMessage("POL is required.");

            // POW and Cut Off are optional for Export

            // Import fields are not allowed for Export
            RuleFor(x => x.MBLNumber)
                .Null()
                .WithMessage("M/BL Number is not allowed for Export.");

            RuleFor(x => x.Destination)
                .Null()
                .WithMessage("Destination is not allowed for Export.");

            RuleFor(x => x.PODId)
                .Null()
                .WithMessage("POD is not allowed for Export.");

            RuleFor(x => x.FreeTimeTill)
                .Null()
                .WithMessage("Free Time Till is not allowed for Export.");

            RuleFor(x => x.RequiredOffloadingDate)
                .Null()
                .WithMessage("Required Offloading Date is not allowed for Export.");

            RuleFor(x => x.RequiredOffloadingTime)
                .Null()
                .WithMessage("Required Offloading Time is not allowed for Export.");
        });
    }
}
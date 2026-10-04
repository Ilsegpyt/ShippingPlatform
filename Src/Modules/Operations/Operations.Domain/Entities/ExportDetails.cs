using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class ExportDetails : SoftDeletableEntity<Guid>
{
    public Guid OperationId { get; private set; }

    public ClearanceType ClearanceType { get; private set; }

    public string BookingNumber { get; private set; } = null!;

    public Guid POLId { get; private set; }

    public Port POL { get; private set; } = null!;

    public Guid? POWId { get; private set; }

    public Port? POW { get; private set; }

    public DateOnly? CutOffDate { get; private set; }

    public string? Customs { get; private set; }

    public Operation Operation { get; private set; } = null!;

    private ExportDetails()
    {
    }
}
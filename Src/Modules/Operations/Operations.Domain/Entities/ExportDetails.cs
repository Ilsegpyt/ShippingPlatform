using BuildingBlocks.Domain;
using Operations.Domain.Enums;
using Operations.Domain.Common;

namespace Operations.Domain.Entities;

public class ExportDetails : Entity<Guid>
{
    public Guid OperationId { get; private set; }

    public ClearanceType ClearanceType { get; private set; }

    public string BookingNumber { get; private set; } = null!;

    public Guid POLId { get; private set; }

    public Port POL { get; private set; } = null!;

    public Guid? PODId { get; private set; }

    public Port? POD { get; private set; }

    public Guid? POWId { get; private set; }

    public Port? POW { get; private set; }

    public DateOnly? CutOffDate { get; private set; }

    public string? Customs { get; private set; }

    public Operation Operation { get; private set; } = null!;

    private ExportDetails()
    {
    }

    public void UpdateCustoms(string? customs)
    {
        Customs = string.IsNullOrWhiteSpace(customs)
            ? null
            : customs.Trim();
    }

    public void UpdatePOL(Guid polId)
    {
        POLId = polId;
    }

    public void UpdatePOD(Guid? podId)
    {
        PODId = podId;
    }

    public void UpdatePOW(Guid? powId)
    {
        POWId = powId;
    }

    public ExportDetails(
        ClearanceType clearanceType,
        string bookingNumber,
        Guid polId,
        Guid? powId,
        DateOnly? cutOffDate,
        Guid? podId = null)
    {
        ClearanceType = clearanceType;
        BookingNumber = IdentifierNormalizer.Normalize(bookingNumber) ?? string.Empty;
        POLId = polId;
        POWId = powId;
        CutOffDate = cutOffDate;
        PODId = podId;
    }
}

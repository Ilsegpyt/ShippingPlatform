
using BuildingBlocks.Domain;
using Operations.Domain.Common;

namespace Operations.Domain.Entities;

public class ImportDetails : SoftDeletableEntity<Guid>
{
    public Guid OperationId { get; private set; }

    public Operation Operation { get; private set; } = null!;

    public string MBLNumber { get; private set; } = null!;

    public Guid PODId { get; private set; }

    public Port POD { get; private set; } = null!;

    public DateOnly RequiredOffloadingDate { get; private set; }

    public TimeOnly RequiredOffloadingTime { get; private set; }

    private ImportDetails()
    {
    }

    public ImportDetails(
        string mblNumber,
        Guid podId,
        DateOnly requiredOffloadingDate,
        TimeOnly requiredOffloadingTime)
    {
        MBLNumber = IdentifierNormalizer.Normalize(mblNumber) ?? string.Empty;
        PODId = podId;
        RequiredOffloadingDate = requiredOffloadingDate;
        RequiredOffloadingTime = requiredOffloadingTime;
    }
}

using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class ImportDetails : SoftDeletableEntity<Guid>
{
    public Guid OperationId { get; private set; }

    public Operation Operation { get; private set; } = null!;

    public string MBLNumber { get; private set; } = null!;

    public string Destination { get; private set; } = null!;

    public Guid PODId { get; private set; }

    public Port POD { get; private set; } = null!;

    public DateOnly FreeTimeTill { get; private set; }

    public DateOnly RequiredOffloadingDate { get; private set; }

    public TimeOnly RequiredOffloadingTime { get; private set; }

    private ImportDetails()
    {
    }
}
using BuildingBlocks.Domain;
using Operations.Domain.Common;

namespace Operations.Domain.Entities.Fcl;

public class ImportDetails : Entity<Guid>
{
    public Guid OperationId { get; private set; }

    public Operation Operation { get; private set; } = null!;

    public string MBLNumber { get; private set; } = null!;

    public Guid PODId { get; private set; }

    public Port POD { get; private set; } = null!;

    public Guid? TerminalId { get; private set; }

    public PortTerminal? Terminal { get; private set; }

    public DateOnly RequiredOffloadingDate { get; private set; }

    public TimeOnly RequiredOffloadingTime { get; private set; }

    public void UpdateRequiredOffloading(DateOnly requiredOffloadingDate, TimeOnly requiredOffloadingTime)
    {
        RequiredOffloadingDate = requiredOffloadingDate;
        RequiredOffloadingTime = requiredOffloadingTime;
    }

    public void UpdateTerminal(Guid? terminalId)
    {
        TerminalId = terminalId;
    }

    private ImportDetails()
    {
    }

    public ImportDetails(
        string mblNumber,
        Guid podId,
        DateOnly requiredOffloadingDate,
        TimeOnly requiredOffloadingTime,
        Guid? terminalId = null)
    {
        MBLNumber = IdentifierNormalizer.Normalize(mblNumber) ?? string.Empty;
        PODId = podId;
        TerminalId = terminalId;
        RequiredOffloadingDate = requiredOffloadingDate;
        RequiredOffloadingTime = requiredOffloadingTime;
    }
}

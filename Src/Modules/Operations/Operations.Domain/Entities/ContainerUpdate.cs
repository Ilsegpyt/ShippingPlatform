namespace Operations.Domain.Entities;

public class ContainerUpdate
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid OperationContainerId { get; private set; }

    public string Details { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public string CreatedByUserId { get; private set; } = null!;

    public OperationContainer OperationContainer { get; private set; } = null!;

    private ContainerUpdate()
    {
    }
}
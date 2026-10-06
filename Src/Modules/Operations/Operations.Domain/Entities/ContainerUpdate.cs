namespace Operations.Domain.Entities;

public class ContainerUpdate
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid OperationContainerId { get; private set; }

    public string Details { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public string CreatedByUserId { get; private set; } = null!;

    public string CreatedByName { get; private set; } = null!;

    public OperationContainer OperationContainer { get; private set; } = null!;

    private ContainerUpdate()
    {
    }

    public static ContainerUpdate Create(
        Guid operationContainerId,
        string details,
        string userId,
        string userName,
        DateTime utcNow)
    {
        var update = new ContainerUpdate
        {
            OperationContainerId = operationContainerId,
            Details = details
        };

        update.SetCreatedAudit(
            userId,
            userName,
            utcNow);

        return update;
    }

    public void SetCreatedAudit(
        string userId,
        string userName,
        DateTime utcNow)
    {
        CreatedByUserId = userId;
        CreatedByName = userName;
        CreatedAtUtc = utcNow;
    }
}
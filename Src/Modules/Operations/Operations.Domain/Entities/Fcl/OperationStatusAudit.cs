using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

/// <summary>Append-only history of changes to Operations.Status only.</summary>
public sealed class OperationStatusAudit
{
    public long Id { get; private set; }
    public Guid OperationId { get; private set; }
    public Operation Operation { get; private set; } = null!;
    public OperationStatus BeforeStatus { get; private set; }
    public OperationStatus AfterStatus { get; private set; }
    public string UserId { get; private set; } = null!;
    public DateTime AtUtc { get; private set; }

    private OperationStatusAudit() { }

    public OperationStatusAudit(Guid operationId, OperationStatus beforeStatus, OperationStatus afterStatus, string userId, DateTime atUtc)
    {
        if (operationId == Guid.Empty) throw new ArgumentException("Operation is required.", nameof(operationId));
        if (beforeStatus == afterStatus) throw new ArgumentException("A status audit requires an actual status change.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.", nameof(userId));
        if (atUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Audit timestamp must be UTC.", nameof(atUtc));

        OperationId = operationId;
        BeforeStatus = beforeStatus;
        AfterStatus = afterStatus;
        UserId = userId.Trim();
        AtUtc = atUtc;
    }
}

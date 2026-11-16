using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public sealed class FtlStatusAudit
{
    public long Id { get; private set; }
    public Guid FtlOperationId { get; private set; }
    public FtlOperation Operation { get; private set; } = null!;
    public OperationStatus BeforeStatus { get; private set; }
    public OperationStatus AfterStatus { get; private set; }
    public string UserId { get; private set; } = null!;
    public DateTime AtUtc { get; private set; }

    private FtlStatusAudit() { }

    public FtlStatusAudit(Guid ftlOperationId, OperationStatus beforeStatus, OperationStatus afterStatus, string userId, DateTime atUtc)
    {
        if (ftlOperationId == Guid.Empty) throw new ArgumentException("Operation is required.", nameof(ftlOperationId));
        if (beforeStatus == afterStatus) throw new ArgumentException("A status audit requires an actual status change.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.", nameof(userId));
        if (atUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Audit timestamp must be UTC.", nameof(atUtc));

        FtlOperationId = ftlOperationId;
        BeforeStatus = beforeStatus;
        AfterStatus = afterStatus;
        UserId = userId.Trim();
        AtUtc = atUtc;
    }
}


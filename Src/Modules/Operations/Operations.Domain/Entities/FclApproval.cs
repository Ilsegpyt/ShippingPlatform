
namespace Operations.Domain.Entities;

/// <summary>
/// Durable record of an approved FCL suggestion and the newly created
/// Export operation containing exactly one FCL container.
/// </summary>
public sealed class FclApproval
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ImportContainerId { get; private set; }
    public OperationContainer ImportContainer { get; private set; } = null!;

    public Guid? ExportOperationId { get; private set; }
    public Operation? ExportOperation { get; private set; }

    /// <summary>
    /// Normalized CNTR|M/BL|BOOKING.
    /// Unique across all users and shipping lines.
    /// </summary>
    public string ApprovalKey { get; private set; } = null!;

    public string ApprovedByUserId { get; private set; } = null!;

    public DateTime ApprovedAtUtc { get; private set; }

    private FclApproval()
    {
    }

    public FclApproval(
        Guid importContainerId,
        string approvalKey,
        string approvedByUserId,
        DateTime approvedAtUtc,
        Guid? exportOperationId = null)
    {
        if (importContainerId == Guid.Empty)
            throw new ArgumentException(
                "Import container is required.",
                nameof(importContainerId));

        if (string.IsNullOrWhiteSpace(approvalKey))
            throw new ArgumentException(
                "Approval key is required.",
                nameof(approvalKey));

        if (string.IsNullOrWhiteSpace(approvedByUserId))
            throw new ArgumentException(
                "Approving user is required.",
                nameof(approvedByUserId));

        if (approvedAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "Approval timestamp must be UTC.",
                nameof(approvedAtUtc));

        ImportContainerId = importContainerId;
        ApprovalKey = approvalKey.Trim();
        ApprovedByUserId = approvedByUserId.Trim();
        ApprovedAtUtc = approvedAtUtc;
        ExportOperationId = exportOperationId;
    }
}


namespace Operations.Domain.Entities;

/// <summary>
/// Durable record of an approved FCL suggestion and the newly created
/// Export operation containing exactly one FCL container.
/// </summary>
public sealed class FclApproval
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid SuggestionId { get; private set; }
    public FclSuggestion Suggestion { get; private set; } = null!;

    public Guid ExportOperationId { get; private set; }
    public Operation ExportOperation { get; private set; } = null!;

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
        Guid suggestionId,
        Guid exportOperationId,
        string approvalKey,
        string approvedByUserId,
        DateTime approvedAtUtc)
    {
        if (suggestionId == Guid.Empty)
            throw new ArgumentException(
                "Suggestion is required.",
                nameof(suggestionId));

        if (exportOperationId == Guid.Empty)
            throw new ArgumentException(
                "New Export operation is required.",
                nameof(exportOperationId));

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

        SuggestionId = suggestionId;
        ExportOperationId = exportOperationId;
        ApprovalKey = approvalKey.Trim();
        ApprovedByUserId = approvedByUserId.Trim();
        ApprovedAtUtc = approvedAtUtc;
    }
}

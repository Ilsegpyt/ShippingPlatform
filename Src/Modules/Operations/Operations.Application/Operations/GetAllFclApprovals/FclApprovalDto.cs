namespace Operations.Application.Operations.GetAllFclApprovals;

public sealed record FclApprovalDto(
    Guid Id,
    Guid SuggestionId,
    Guid ExportOperationId,
    string ApprovalKey,
    string ApprovedByUserId,
    DateTime ApprovedAtUtc);
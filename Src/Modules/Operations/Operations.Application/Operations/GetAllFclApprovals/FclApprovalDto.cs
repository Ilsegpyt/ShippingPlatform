namespace Operations.Application.Operations.GetAllFclApprovals;

public sealed record FclApprovalDto(
    Guid Id,
    Guid ImportContainerId,
    Guid? ExportOperationId,
    string ApprovalKey,
    string ApprovedByUserId,
    DateTime ApprovedAtUtc);
using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.GetAllFclApprovals;

public sealed class GetAllFclApprovalsQueryHandler(
    IFclRepository repository)
    : IRequestHandler<
        GetAllFclApprovalsQuery,
        Result<IReadOnlyList<FclApprovalDto>>>
{
    public async Task<Result<IReadOnlyList<FclApprovalDto>>> Handle(
        GetAllFclApprovalsQuery query,
        CancellationToken cancellationToken)
    {
        var approvals = await repository.GetAllApprovalsAsync(
            cancellationToken);

        IReadOnlyList<FclApprovalDto> response = approvals
            .OrderByDescending(x => x.ApprovedAtUtc)
            .Select(x => new FclApprovalDto(
                x.Id,
                x.SuggestionId,
                x.ExportOperationId,
                x.ApprovalKey,
                x.ApprovedByUserId,
                x.ApprovedAtUtc))
            .ToList();

        return Result.Success(response);
    }
}
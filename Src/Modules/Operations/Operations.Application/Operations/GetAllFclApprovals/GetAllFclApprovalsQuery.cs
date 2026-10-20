using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.GetAllFclApprovals;

public sealed record GetAllFclApprovalsQuery
    : IRequest<Result<IReadOnlyList<FclApprovalDto>>>;
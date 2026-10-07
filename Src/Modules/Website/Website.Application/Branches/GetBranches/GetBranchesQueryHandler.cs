using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.Branches.GetBranches;

public sealed class GetBranchesQueryHandler(
    IBranchRepository branchRepository)
    : IRequestHandler<
        GetBranchesQuery,
        Result<IReadOnlyList<Branch>>>
{
    public async Task<Result<IReadOnlyList<Branch>>> Handle(
        GetBranchesQuery query,
        CancellationToken ct)
    {
        var branches = await branchRepository.GetAllAsync(ct);

        return Result.Success(branches);
    }
}
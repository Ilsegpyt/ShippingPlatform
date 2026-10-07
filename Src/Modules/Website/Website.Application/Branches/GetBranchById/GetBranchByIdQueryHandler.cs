using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.Branches.GetBranchById;

public sealed class GetBranchByIdQueryHandler(
    IBranchRepository branchRepository)
    : IRequestHandler<
        GetBranchByIdQuery,
        Result<Branch>>
{
    public async Task<Result<Branch>> Handle(
        GetBranchByIdQuery query,
        CancellationToken ct)
    {
        var branch =
            await branchRepository.GetByIdAsync(
                query.Id,
                ct);

        if (branch is null)
            return Result.Failure<Branch>(
                "Branch was not found.");

        return Result.Success(branch);
    }
}
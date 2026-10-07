using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.Branches.RestoreBranch;

public sealed class RestoreBranchCommandHandler(
    IBranchRepository branchRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<RestoreBranchCommand, Result>
{
    public async Task<Result> Handle(
        RestoreBranchCommand cmd,
        CancellationToken ct)
    {
        var branch = await branchRepository.GetForUpdateAsync(cmd.Id, ct);

        if (branch is null)
            return Result.Failure("Branch was not found.");

        branch.IsActive = true;

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.Branches.DeleteBranch;

public sealed class DeleteBranchCommandHandler(
    IBranchRepository branchRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<DeleteBranchCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBranchCommand cmd,
        CancellationToken ct)
    {
        var branch = await branchRepository.GetForUpdateAsync(cmd.Id, ct);

        if (branch is null)
            return Result.Failure("Branch was not found.");

        branch.IsActive = false;

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.Branches.UpdateBranch;

public sealed class UpdateBranchCommandHandler(
    IBranchRepository branchRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<UpdateBranchCommand, Result>
{
    public async Task<Result> Handle(
        UpdateBranchCommand cmd,
        CancellationToken ct)
    {
        var branch = await branchRepository.GetForUpdateAsync(cmd.Id, ct);

        if (branch is null)
            return Result.Failure("Branch was not found.");

        branch.Name = cmd.Name;
        branch.Address = cmd.Address;
        branch.Latitude = cmd.Latitude;
        branch.Longitude = cmd.Longitude;
        branch.CeoName = cmd.CeoName;
        branch.CeoEmail = cmd.CeoEmail;
        branch.GmName = cmd.GmName;
        branch.GmEmail = cmd.GmEmail;
        branch.BranchManagerName = cmd.BranchManagerName;
        branch.BranchManagerEmail = cmd.BranchManagerEmail;
        branch.IsActive = cmd.IsActive;

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
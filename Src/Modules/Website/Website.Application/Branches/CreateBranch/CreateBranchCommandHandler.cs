using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.Branches.CreateBranch;

public sealed class CreateBranchCommandHandler(
    IBranchRepository branchRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<CreateBranchCommand, Result>
{
    public async Task<Result> Handle(
        CreateBranchCommand cmd,
        CancellationToken ct)
    {
        var branch = new Branch
        {
            Name = cmd.Name,
            Address = cmd.Address,
            Latitude = cmd.Latitude,
            Longitude = cmd.Longitude,
            CeoName = cmd.CeoName,
            CeoEmail = cmd.CeoEmail,
            GmName = cmd.GmName,
            GmEmail = cmd.GmEmail,
            BranchManagerName = cmd.BranchManagerName,
            BranchManagerEmail = cmd.BranchManagerEmail,
            IsActive = cmd.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        branchRepository.Add(branch);

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
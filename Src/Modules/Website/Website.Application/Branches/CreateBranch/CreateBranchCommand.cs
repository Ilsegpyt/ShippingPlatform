using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.Branches.CreateBranch;

public sealed record CreateBranchCommand(
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string CeoName,
    string CeoEmail,
    string GmName,
    string GmEmail,
    string BranchManagerName,
    string BranchManagerEmail,
    bool IsActive)
    : IRequest<Result>;
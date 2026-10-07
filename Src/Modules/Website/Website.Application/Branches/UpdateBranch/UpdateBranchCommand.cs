using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.Branches.UpdateBranch;

public sealed record UpdateBranchCommand(
    int Id,
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string? CeoName,
    string? CeoEmail,
    string? GmName,
    string? GmEmail,
    string? BranchManagerName,
    string? BranchManagerEmail,
    bool IsActive)
    : IRequest<Result>;
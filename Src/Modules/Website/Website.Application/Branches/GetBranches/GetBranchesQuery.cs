using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.Branches.GetBranches;

public sealed record GetBranchesQuery
    : IRequest<Result<IReadOnlyList<Branch>>>;
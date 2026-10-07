using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.Branches.GetBranchById;

public sealed record GetBranchByIdQuery(
    int Id)
    : IRequest<Result<Branch>>;
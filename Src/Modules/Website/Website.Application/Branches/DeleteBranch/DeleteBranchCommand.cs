using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.Branches.DeleteBranch;

public sealed record DeleteBranchCommand(int Id)
    : IRequest<Result>;
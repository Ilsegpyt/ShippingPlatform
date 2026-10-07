using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.Branches.RestoreBranch;

public sealed record RestoreBranchCommand(int Id)
    : IRequest<Result>;
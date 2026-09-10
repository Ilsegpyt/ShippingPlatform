using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountProfile;

public sealed record UpdateSubAccountProfileCommand(
    Guid CustomerId,
    Guid SubAccountId,
    string Name) : IRequest<Result>;
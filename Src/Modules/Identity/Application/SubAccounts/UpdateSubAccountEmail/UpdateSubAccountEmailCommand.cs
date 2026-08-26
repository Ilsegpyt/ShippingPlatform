using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountEmail;

public sealed record UpdateSubAccountEmailCommand(
    Guid SubAccountId,
    string Email) : IRequest<Result>;
using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeliverFtlOperation;

public sealed record DeliverFtlOperationCommand(string Reference)
    : IRequest<Result<bool>>;

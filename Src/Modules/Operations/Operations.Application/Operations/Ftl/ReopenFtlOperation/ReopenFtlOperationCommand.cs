using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.ReopenFtlOperation;

public sealed record ReopenFtlOperationCommand(string Reference, string? Reason = null)
    : IRequest<Result<bool>>;

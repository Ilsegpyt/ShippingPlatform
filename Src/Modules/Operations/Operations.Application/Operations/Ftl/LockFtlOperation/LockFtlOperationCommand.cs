using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.LockFtlOperation;

public sealed record LockFtlOperationCommand(string Ref) : IRequest<Result<LockFtlOperationResult>>;

public sealed record LockFtlOperationResult(bool IsLocked, string? LockedByName, DateTime? LockedAtUtc);

using System;

namespace Operations.Application.Operations.LockOperation;

public sealed record OperationLockDto(
    bool IsLocked,
    Guid? LockedByUserId,
    string? LockedByName,
    DateTime? LockedAtUtc);


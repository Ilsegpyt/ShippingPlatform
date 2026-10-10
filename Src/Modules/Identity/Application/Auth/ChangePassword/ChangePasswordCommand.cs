
using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Auth.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword)
    : IRequest<Result<ChangePasswordResponse>>;

public sealed record ChangePasswordResponse(bool Changed);

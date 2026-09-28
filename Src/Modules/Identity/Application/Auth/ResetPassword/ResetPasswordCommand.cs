using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Auth.ResetPassword;

public sealed record ResetPasswordCommand(
    Guid UserId,
    string ResetToken,
    string NewPassword
) : IRequest<Result>;
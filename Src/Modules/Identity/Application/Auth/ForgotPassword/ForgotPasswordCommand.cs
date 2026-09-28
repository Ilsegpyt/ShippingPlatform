using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email
) : IRequest<Result>;
using BuildingBlocks.Application;
using MediatR;
namespace Identity.Application.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

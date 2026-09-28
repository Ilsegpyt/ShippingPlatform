using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Identity.Application.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityUserService _identityUsers;
    private readonly IPasswordResetEmailSender _passwordResetEmailSender;
    private readonly ForgotPasswordOptions _options;

    public ForgotPasswordCommandHandler(
        IIdentityUserService identityUsers,
        IPasswordResetEmailSender passwordResetEmailSender,
        IOptions<ForgotPasswordOptions> options)
    {
        _identityUsers = identityUsers;
        _passwordResetEmailSender = passwordResetEmailSender;
        _options = options.Value;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken ct)
    {
        var result =
            await _identityUsers.GeneratePasswordResetTokenAsync(
                request.Email,
                ct);

        if (result is null)
        {
            // Do not reveal whether the email exists.
            return Result.Success();
        }

        var resetLink =
            $"{_options.FrontendBaseUrl.TrimEnd('/')}" +
            $"/reset-password" +
            $"?userId={result.Value.UserId}" +
            $"&token={Uri.EscapeDataString(result.Value.ResetToken)}";

        Console.WriteLine($"PASSWORD RESET LINK: {resetLink}");

        await _passwordResetEmailSender.SendAsync(
            result.Value.Email,
            resetLink,
            ct);

        return Result.Success();
    }
}
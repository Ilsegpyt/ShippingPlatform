using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly ITokenService _tokens;

    public RefreshTokenHandler(ITokenService tokens) => _tokens = tokens;

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var pair = await _tokens.RefreshAsync(request.RefreshToken, ct);
        if (pair is null)
            return Result.Failure<RefreshTokenResponse>("Invalid or expired refresh token.");

        return Result.Success(new RefreshTokenResponse(pair.AccessToken, pair.RefreshToken, pair.AccessTokenExpiresAtUtc));
    }
}

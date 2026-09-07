using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly ITokenService _tokens;
    private readonly IIdentityUnitOfWork _identityUitOfWork;

    public RefreshTokenHandler(ITokenService tokens, IIdentityUnitOfWork unitOfWork) => (_tokens, _identityUitOfWork) = (tokens, unitOfWork);

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var pair = await _tokens.RefreshAsync(request.RefreshToken, ct);
        if (pair is null)
            return Result.Failure<RefreshTokenResponse>("Invalid or expired refresh token.");

        await _identityUitOfWork.SaveChangesAsync(ct);

        return Result.Success(new RefreshTokenResponse(pair.AccessToken, pair.RefreshToken, pair.AccessTokenExpiresAtUtc));
    }
}
using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Auth.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IIdentityUserService _identityUsers;
    private readonly ITokenService _tokens;
    private readonly TokenClaimsBuilder _claimsBuilder;

    public LoginHandler(IIdentityUserService identityUsers, ITokenService tokens, TokenClaimsBuilder claimsBuilder)
    {
        _identityUsers = identityUsers;
        _tokens = tokens;
        _claimsBuilder = claimsBuilder;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var userId = await _identityUsers.ValidateCredentialsAsync(request.Email, request.Password, ct);
        if (userId is null)
            return Result.Failure<LoginResponse> ("Invalid email or password.");



        var claimsResult = await _claimsBuilder.BuildAsync(userId.Value, ct);



        if (claimsResult.IsFailure) 
            return Result.Failure<LoginResponse>(claimsResult.Error!);

        var tokenPair = await _tokens.IssueTokensAsync(userId.Value, claimsResult.Value, ct);





        return Result.Success(new LoginResponse(
            tokenPair.AccessToken,
            tokenPair.RefreshToken,
            tokenPair.AccessTokenExpiresAtUtc));
    }
}
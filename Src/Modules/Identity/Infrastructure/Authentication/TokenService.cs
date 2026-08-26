using Identity.Application;
using Identity.Application.Abstractions;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Authentication;

/// <summary>
/// Handles the creation, rotation, and revocation of authentication tokens.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly IdentityDbContext _db;
    private readonly JwtOptions _options;
    private readonly IIdentityUserService _identityUsers;
    private readonly TokenClaimsBuilder _claimsBuilder;

    public TokenService(IdentityDbContext db, IOptions<JwtOptions> options, IIdentityUserService identityUsers, TokenClaimsBuilder claimsBuilder)
    {
        _db = db;
        _options = options.Value;
        _identityUsers = identityUsers;
        _claimsBuilder = claimsBuilder;
    }

    /// <summary>
    /// Creates an access token and a refresh token for the specified user.
    /// </summary>
    public async Task<TokenPair> IssueTokensAsync(Guid userId, IReadOnlyDictionary<string, string> claims, CancellationToken ct = default)
    {
        var (accessToken, expiresAtUtc) = CreateAccessToken(userId, claims);
        var refreshTokenPlain = GenerateSecureRandomToken();

        // Store only the hashed refresh token in the database.
        var refreshToken = RefreshToken.Create(
            userId,
            Hash(refreshTokenPlain),
            DateTime.UtcNow.AddDays(_options.RefreshTokenDays));


        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(ct);

        // Return the plain refresh token only to the caller.
        return new TokenPair(accessToken, refreshTokenPlain, expiresAtUtc);
    }

    /// <summary>
    /// Validates and rotates a refresh token, returning a new token pair.
    /// </summary>
    /// // Edited
    public async Task<TokenPair?> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        var hash = Hash(refreshToken);

        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

        if (existing is null || !existing.IsActive)
            return null;

        // Do not issue new tokens for an inactive user.
        var userIsActive = await _identityUsers.IsActiveAsync(
            existing.UserId,
            ct);

        if (!userIsActive)
            return null;

        // Rebuild the claims from the current business state.
        var claimsResult = await _claimsBuilder.BuildAsync(
            existing.UserId,
            ct);

        if (claimsResult.IsFailure)
            return null;

        // Rotation: revoke the used refresh token and issue a new token pair.
        var (accessToken, expiresAtUtc) = CreateAccessToken(
            existing.UserId,
            claimsResult.Value);

        var newRefreshTokenPlain = GenerateSecureRandomToken();

        // Store only the hash of the new refresh token.
        var newRefreshToken = RefreshToken.Create(
            existing.UserId,
            Hash(newRefreshTokenPlain),
            DateTime.UtcNow.AddDays(_options.RefreshTokenDays));


        // Link the old token to the newly issued token before revoking it.
        existing.Revoke(newRefreshToken.Id);

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync(ct);

        return new TokenPair(
            accessToken,
            newRefreshTokenPlain,
            expiresAtUtc);
    }
    /// <summary>
    /// Revokes the specified refresh token so it can no longer be used.
    /// </summary>
    public async Task RevokeAsync(string refreshToken, CancellationToken ct = default)
    {
        var hash = Hash(refreshToken);

        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

        existing?.Revoke();

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Creates and signs a JWT access token containing the user's claims.
    /// </summary>
    private (string token, DateTime expiresAtUtc) CreateAccessToken(Guid userId, IReadOnlyDictionary<string, string> claims)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        // The subject claim always comes from the user ID parameter.
        var claimsList = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString())
        };

        // Add additional claims while preventing the caller from overriding "sub".
        claimsList.AddRange(
            claims
                .Where(c => c.Key != "sub")
                .Select(c => new Claim(c.Key, c.Value)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claimsList,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }

    /// <summary>
    /// Generates a cryptographically secure random refresh token.
    /// </summary>
    private static string GenerateSecureRandomToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    /// <summary>
    /// Creates a SHA-256 hash used to store refresh tokens securely.
    /// </summary>
    private static string Hash(string value) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
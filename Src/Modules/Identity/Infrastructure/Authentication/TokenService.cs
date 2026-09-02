using BuildingBlocks.Application.Contracts;
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

public sealed class TokenService : ITokenService
{
    private readonly IdentityDbContext _db;
    private readonly JwtOptions _options;
    private readonly IIdentityUserService _identityUsers;
    private readonly TokenClaimsBuilder _claimsBuilder;
    private readonly ICustomerQueries _customers;

    public TokenService(
        IdentityDbContext db,
        IOptions<JwtOptions> options,
        IIdentityUserService identityUsers,
        TokenClaimsBuilder claimsBuilder,
        ICustomerQueries customers)
    {
        _db = db;
        _options = options.Value;
        _identityUsers = identityUsers;
        _claimsBuilder = claimsBuilder;
        _customers = customers;
    }

    public async Task<TokenPair> IssueTokensAsync(
        Guid userId,
        IReadOnlyDictionary<string, string> claims,
        CancellationToken ct = default)
    {
        var (accessToken, expiresAtUtc) = CreateAccessToken(
            userId,
            claims);

        var refreshTokenPlain = GenerateSecureRandomToken();

        var refreshToken = RefreshToken.Create(
            userId,
            Hash(refreshTokenPlain),
            DateTime.UtcNow.AddDays(_options.RefreshTokenDays));

        _db.RefreshTokens.Add(refreshToken);

        await _db.SaveChangesAsync(ct);

        return new TokenPair(
            accessToken,
            refreshTokenPlain,
            expiresAtUtc);
    }

    /// <summary>
    /// Issues a new access and refresh token pair for a customer impersonation session.
    /// The impersonator remains the authenticated user while the target organization
    /// is stored as the impersonation context.
    /// </summary>
    public async Task<TokenPair> IssueImpersonationTokensAsync(
        Guid impersonatorUserId,
        Guid impersonatedOrganizationId,
        CancellationToken ct = default)
    {
        var claims = new Dictionary<string, string>
        {
            ["token_type"] = "impersonation",
            ["org_id"] = impersonatedOrganizationId.ToString()
        };

        var (accessToken, expiresAtUtc) = CreateAccessToken(
            impersonatorUserId,
            claims);

        var refreshTokenPlain = GenerateSecureRandomToken();

        var refreshToken = RefreshToken.CreateImpersonation(
            impersonatorUserId,
            Hash(refreshTokenPlain),
            DateTime.UtcNow.AddDays(_options.RefreshTokenDays),
            impersonatedOrganizationId);

        _db.RefreshTokens.Add(refreshToken);

        await _db.SaveChangesAsync(ct);

        return new TokenPair(
            accessToken,
            refreshTokenPlain,
            expiresAtUtc);
    }

    public async Task<TokenPair?> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        var hash = Hash(refreshToken);

        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == hash,
                ct);

        if (existing is null || !existing.IsActive)
            return null;

        var userIsActive = await _identityUsers.IsActiveAsync(
            existing.UserId,
            ct);

        if (!userIsActive)
            return null;

        Dictionary<string, string> claims;

        if (existing.TokenType == "impersonation")
        {
            if (existing.ImpersonatedOrganizationId is null)
                return null;

            var customer = await _customers.GetForAssignmentAsync(
                existing.ImpersonatedOrganizationId.Value,
                ct);

            if (customer is null || !customer.IsActive)
                return null;

            claims = new Dictionary<string, string>
            {
                ["token_type"] = "impersonation",
                ["org_id"] =
                    existing.ImpersonatedOrganizationId.Value.ToString()
            };
        }
        else
        {
            var claimsResult = await _claimsBuilder.BuildAsync(
                existing.UserId,
                ct);

            if (claimsResult.IsFailure)
                return null;

            claims = claimsResult.Value;
        }

        var (accessToken, expiresAtUtc) = CreateAccessToken(
            existing.UserId,
            claims);

        var newRefreshTokenPlain = GenerateSecureRandomToken();

        RefreshToken newRefreshToken;

        if (existing.TokenType == "impersonation")
        {
            newRefreshToken = RefreshToken.CreateImpersonation(
                existing.UserId,
                Hash(newRefreshTokenPlain),
                DateTime.UtcNow.AddDays(_options.RefreshTokenDays),
                existing.ImpersonatedOrganizationId!.Value);
        }
        else
        {
            newRefreshToken = RefreshToken.Create(
                existing.UserId,
                Hash(newRefreshTokenPlain),
                DateTime.UtcNow.AddDays(_options.RefreshTokenDays));
        }

        existing.Revoke(newRefreshToken.Id);

        _db.RefreshTokens.Add(newRefreshToken);

        await _db.SaveChangesAsync(ct);

        return new TokenPair(
            accessToken,
            newRefreshTokenPlain,
            expiresAtUtc);
    }

    public async Task RevokeAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        var hash = Hash(refreshToken);

        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == hash,
                ct);

        existing?.Revoke();

        await _db.SaveChangesAsync(ct);
    }

    private (
        string token,
        DateTime expiresAtUtc) CreateAccessToken(
        Guid userId,
        IReadOnlyDictionary<string, string> claims)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(
            _options.AccessTokenMinutes);

        var claimsList = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                userId.ToString())
        };

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

        return (
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }

    private static string GenerateSecureRandomToken() =>
        Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

    private static string Hash(string value) =>
        Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(value)));
}
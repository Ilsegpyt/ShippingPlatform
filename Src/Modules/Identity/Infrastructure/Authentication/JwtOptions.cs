namespace Identity.Infrastructure.Authentication;

/// <summary>
/// Strongly typed JWT configuration, bound from appsettings via the Options Pattern
/// (registered with services.AddOptions&lt;JwtOptions&gt;().Bind(...).ValidateDataAnnotations()).
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public int AccessTokenMinutes { get; init; } = 30;
    public int RefreshTokenDays { get; init; } = 14;
}

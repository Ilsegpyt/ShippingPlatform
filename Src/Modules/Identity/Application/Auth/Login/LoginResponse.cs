namespace Identity.Application.Auth.Login;

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);

namespace Identity.Contracts;

public interface IIdentityUserRegistrar
{
    Task<Guid> CreateUserAsync(string email, CancellationToken ct);
    string GetDefaultPassword();
}
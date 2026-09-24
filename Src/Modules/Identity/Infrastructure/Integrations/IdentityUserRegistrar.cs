using Identity.Application.Abstractions;
using Identity.Contracts;

namespace Identity.Infrastructure.Integrations;

internal sealed class IdentityUserRegistrar(
    IIdentityUserService identityUsers) : IIdentityUserRegistrar
{
    public async Task<Guid> CreateUserAsync(
        string email,
        CancellationToken ct)
    {
        return await identityUsers.CreateUserAsync(
            email,
            isInternal: false,
            phone: null,
            ct);
    }

    // TODO: Re-enable/remove this when the customer activation flow is completed.
    public string GetDefaultPassword()
        => throw new NotImplementedException(
            "Default passwords are temporarily disabled. Use the activation flow.");
}
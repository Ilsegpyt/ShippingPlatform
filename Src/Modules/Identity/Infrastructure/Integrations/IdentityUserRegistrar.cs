using Identity.Application.Abstractions;
using Identity.Application.Options;
using Identity.Contracts;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Integrations;

internal sealed class IdentityUserRegistrar(IIdentityUserService identityUsers, IOptions<SubAccountOptions> options) : IIdentityUserRegistrar
{
    private readonly SubAccountOptions _options = options.Value;

    public async Task<Guid> CreateUserAsync( string email, CancellationToken ct)
    {
        return await identityUsers.CreateUserAsync(email, _options.DefaultPassword, isInternal: false, null, ct);
    }

    public string GetDefaultPassword() => _options.DefaultPassword; 
}
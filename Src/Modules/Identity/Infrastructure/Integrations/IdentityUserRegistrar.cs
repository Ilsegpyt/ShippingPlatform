using BuildingBlocks.Application.Contracts;
using Identity.Application;
using Identity.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Integrations;
// Adapter/Integration Between Identity and the other modules , cause he exposes the internal service of
// Identity as external contarct
internal sealed class IdentityUserRegistrar(IIdentityUserService identityUsers, IOptions<SubAccountOptions> options) : IIdentityUserRegistrar
{
    private readonly SubAccountOptions _options = options.Value;

    public async Task<Guid> CreateUserAsync( string email, CancellationToken ct)
    {
        return await identityUsers.CreateUserAsync(email, _options.DefaultPassword, isInternal: false, null, ct);
    }

    public string GetDefaultPassword() => _options.DefaultPassword; 
}
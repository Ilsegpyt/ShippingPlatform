using Identity.Domain;

namespace Api.Modules.Identity.SubAccounts;

public sealed record AddScopeRequestBody(
    ScopeCategory Category,
    ScopeService Service,
    ScopeShipmentType Type);

public sealed record GrantSubAccountPermissionRequestBody(
    string PermissionKey);

public sealed record CreateSubAccountRequestBody(
    string Name,
    string Email,
    bool GrantFullScope,
    IReadOnlyList<AddScopeRequestBody> Scopes);

public sealed record UpdateSubAccountProfileRequestBody(
    string Name);
public sealed record UpdateSubAccountEmailRequestBody(
    string Email);

public sealed record ResetSubAccountPasswordRequestBody(
    string NewPassword); 
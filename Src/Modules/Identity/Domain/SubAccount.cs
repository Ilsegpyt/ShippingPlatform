using BuildingBlocks.Domain;
using Identity.Domain.Events;
using Identity.Domain.Exceptions;

namespace Identity.Domain;

/// <summary>
/// Represents a customer sub-account with its own scopes and permissions.
/// </summary>
public sealed class SubAccount : AggregateRoot<Guid>
{
    private readonly List<PermissionScope> _scopes = new();
    private readonly List<PermissionKey> _permissions = new();

    public string Name { get; private set; } = null!;

    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Links to the ASP.NET Core Identity user.
    /// </summary>
    public Guid UserId { get; private set; }

    public SubAccountStatus Status { get; private set; }

    public ScopeType ScopeType { get; private set; }

    public string Email { get; private set; } = null!;

    public IReadOnlyCollection<PermissionScope> Scopes =>
        _scopes.AsReadOnly();

    public IReadOnlyCollection<PermissionKey> Permissions =>
        _permissions.AsReadOnly();

    // Used by EF Core.
    private SubAccount()
    {
    }

    private SubAccount(Guid id, Guid organizationId, Guid userId, string name, string email, ScopeType scopeType, SubAccountStatus status) : base(id)
    {
        OrganizationId = organizationId;
        UserId = userId;
        ScopeType = scopeType;
        Status = status;
        Name = name;
        Email = email;
    }

    // Create a new sub-account.
    public static SubAccount Create(Guid organizationId, Guid userId, string name, string email, ScopeType scopeType, SubAccountStatus initialStatus)
    {
        var subAccount = new SubAccount(Guid.NewGuid(), organizationId, userId, name,  email, scopeType, initialStatus);

       subAccount.RaiseDomainEvent(
    new SubAccountCreatedEvent( 
        subAccount.Id,
        organizationId,
        name,
        email,
        DateTime.UtcNow));

        return subAccount;
    }
    public void UpdateName(string name)
    {
        Name = name.Trim();
    }

    // Activate the sub-account.
    public void Activate()
    {
        if (Status == SubAccountStatus.Active)
            return;

        Status = SubAccountStatus.Active;

        RaiseDomainEvent(
            new SubAccountStatusChangedEvent(Id, Status, DateTime.UtcNow));
    }

    // Deactivate the sub-account.
    public void Deactivate()
    {
        if (Status == SubAccountStatus.Inactive)
            return;

        Status = SubAccountStatus.Inactive;

        RaiseDomainEvent(
            new SubAccountStatusChangedEvent(Id, Status, DateTime.UtcNow));

    }

    // Switch to full access and remove existing scopes.
    public void SetFullScope()
    {
        ScopeType = ScopeType.Full;
        _scopes.Clear();
    }

    // Switch from full access to custom access.
    public void SetCustomScope()
    {
        ScopeType = ScopeType.Custom;
    }

    // Add a scope to the sub-account.
    public void AddScope(PermissionScope scope)
    {
        EnsureCustomGrant();

        if (_scopes.Contains(scope))
            return;

        _scopes.Add(scope);
    }

    // Remove a scope from the sub-account.
    public void RemoveScope(PermissionScope scope)
    {
        EnsureCustomGrant();

        _scopes.Remove(scope);
    }

    // Grant a permission to the sub-account.
    public void GrantPermission(PermissionKey key)
    {
        if (_permissions.Contains(key))
            return;

        _permissions.Add(key);
    }

    // Check whether the sub-account can view the requested data.
    public bool CanView(ScopeCategory category, ScopeService service, ScopeShipmentType type)
        => ScopeType == ScopeType.Full || IsWithinScope(category, service, type);


    // Revoke a permission from the sub-account.
    public void RevokePermission(PermissionKey key)
    {

        _permissions.Remove(key);
    }

    // Check whether the sub-account has the requested permission.
    public bool HasPermission(PermissionKey key) =>
        //ScopeType == ScopeType.Full || Edited
        _permissions.Contains(key);

    // Check whether the requested data is within the assigned scopes.
    public bool IsWithinScope(ScopeCategory category, ScopeService? service = null, ScopeShipmentType? type = null)
    {
        if (ScopeType == ScopeType.Full)
            return true;
        return _scopes.Any(scope =>
                  scope.Category == category &&
                  MatchesService(scope.Service, service) &&
                  MatchesShipmentType(scope.Type, type));
    }
    private static bool MatchesService(ScopeService scopeService, ScopeService? requestedService)
    {
        if (requestedService is null)
            return true;

        if (scopeService == ScopeService.Both)
            return requestedService is ScopeService.CustomsClearance or ScopeService.Transportation;

        return scopeService == requestedService;
    }
    private static bool MatchesShipmentType(ScopeShipmentType scopeType, ScopeShipmentType? requestedType)
    {
        if (requestedType is null)
            return true;

        if (scopeType == ScopeShipmentType.All)
            return requestedType is ScopeShipmentType.Import or ScopeShipmentType.Export;


        return scopeType == requestedType;
    }

    // Ensure scopes and permissions can only be managed for custom access.
    private void EnsureCustomGrant()
    {
        if (ScopeType != ScopeType.Custom)
        {
            throw new InvalidSubAccountStateException(
                "Custom access can only be managed when ScopeType is Custom.");
        }
    }
    public void UpdateEmail(string email)
    {
        Email = email.Trim();

        RaiseDomainEvent(
            new SubAccountEmailChangedEvent(
                Id,
                UserId,
                Email,
                DateTime.UtcNow));
    }
}
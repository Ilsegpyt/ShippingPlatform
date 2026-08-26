
using BuildingBlocks.Domain;

namespace Identity.Domain;

public enum RoleStatus
{
    Active = 0,
    Inactive = 1
}

/// <summary>
/// Internal (employee-side) role. Unlike Customer SubAccounts (which hold permissions
/// directly), internal Users are granted permissions indirectly through a Role.
/// Roles are dynamic — Super Admin can create new ones from the dashboard, not just
/// the 6 seeded at launch (Super Admin, Account Manager, Operations, Sales, Viewer,
/// Finance Manager).
/// </summary>
public sealed class Role : AggregateRoot<Guid>
{
    private readonly List<PermissionKey> _permissions = new();

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public RoleStatus Status { get; private set; }

    public IReadOnlyCollection<PermissionKey> Permissions => _permissions.AsReadOnly();

    private Role() { }

    private Role(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
        Status = RoleStatus.Active;
    }

    public static Role Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));

        return new Role(Guid.NewGuid(), name.Trim(), description.Trim());
    }

    public void Rename(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));
        Name = name.Trim();
        Description = description.Trim();
    }

    public void Activate() => Status = RoleStatus.Active;
    public void Deactivate() => Status = RoleStatus.Inactive;

    public void GrantPermission(PermissionKey key)
    {
        if (!_permissions.Contains(key))
            _permissions.Add(key);
    }

    public void RevokePermission(PermissionKey key) => _permissions.Remove(key);

    public bool HasPermission(PermissionKey key) => _permissions.Contains(key);
}


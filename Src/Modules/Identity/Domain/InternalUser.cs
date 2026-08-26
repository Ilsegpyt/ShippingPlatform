using BuildingBlocks.Domain;

namespace Identity.Domain;

public enum InternalUserStatus
{
    Active = 0,
    Inactive = 1
}

/// <summary>
/// Business-side representation of an employee (Super Admin, Account Manager, Operations...).
/// Mirrors SubAccount's relationship to authentication: ApplicationUser (Infrastructure) handles
/// login credentials, InternalUser (Domain) holds the RoleId + Status used for authorization.
/// One Role per internal user, matching the current business model.
/// </summary>
public sealed class InternalUser : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Phone { get; private set; }
    public InternalUserStatus Status { get; private set; }

    private InternalUser() { }

    private InternalUser(
        Guid id,
        Guid userId,
        Guid roleId,
        string name,
        string email,
        string? phone) : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        Name = name;
        Email = email;
        Phone = phone;
        Status = InternalUserStatus.Active;
    }

    public static InternalUser Create(
        Guid userId,
        Guid roleId,
        string name,
        string email,
        string? phone)
        => new(
            Guid.NewGuid(),
            userId,
            roleId,
            name,
            email,
            phone);

    public void ChangeRole(Guid roleId)
        => RoleId = roleId;

    public void Activate()
        => Status = InternalUserStatus.Active;

    public void Deactivate()
        => Status = InternalUserStatus.Inactive;

    public void UpdateEmail(string email)
    {
        Email = email.Trim();
    }

    public void UpdateProfile(string name, string? phone)
    {
        Name = name.Trim();
        Phone = phone?.Trim();
    }
}
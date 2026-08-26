
using BuildingBlocks.Domain;

namespace Identity.Domain;

/// <summary>
/// Links an internal User acting as "Account Manager" to a specific Customer they are
/// responsible for. An Account Manager only sees/manages the Customers assigned to them;
/// other Roles (e.g. Super Admin, Operations) are not restricted this way.
/// This is a simple Entity, not an Aggregate Root — it always changes via the assignment
/// use case directly (not through Role or SubAccount).
/// </summary>
public sealed class AccountManagerAssignment : Entity<Guid>
{
    public Guid AccountManagerUserId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }

    private AccountManagerAssignment() { }

    private AccountManagerAssignment(Guid id, Guid accountManagerUserId, Guid customerId) : base(id)
    {
        AccountManagerUserId = accountManagerUserId;
        CustomerId = customerId;
        AssignedAtUtc = DateTime.UtcNow;
    }

    public static AccountManagerAssignment Create(Guid accountManagerUserId, Guid customerId) =>
        new(Guid.NewGuid(), accountManagerUserId, customerId);
}
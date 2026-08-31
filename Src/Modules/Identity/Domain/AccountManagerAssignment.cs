using BuildingBlocks.Domain;

namespace Identity.Domain;

public sealed class AccountManagerAssignment : Entity<Guid>
{
    public Guid AccountManagerId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }

    private AccountManagerAssignment() { }

    private AccountManagerAssignment(
        Guid id,
        Guid accountManagerId,
        Guid customerId) : base(id)
    {
        AccountManagerId = accountManagerId;
        CustomerId = customerId;
        AssignedAtUtc = DateTime.UtcNow;
    }

    public static AccountManagerAssignment Create(
        Guid accountManagerId,
        Guid customerId) =>
        new(Guid.NewGuid(), accountManagerId, customerId);

    public void ChangeAccountManager(Guid accountManagerId)
    {
        AccountManagerId = accountManagerId;
        AssignedAtUtc = DateTime.UtcNow;
    }
}


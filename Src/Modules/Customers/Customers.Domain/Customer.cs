using BuildingBlocks.Domain;
using Customers.Domain.Events;

namespace Customers.Domain;

public sealed class Customer : AggregateRoot<Guid>, ISoftDeletable
{
    public string OwnerName { get; private set; } = null!;
    public string CompanyName { get; private set; } = null!;
    public string OwnerPhone { get; private set; } = null!;
    public string OwnerEmail { get; private set; } = null!;
    public string? Industry { get; private set; }

    public Guid OwnerUserId { get; private set; }
    public CustomerStatus Status { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public void MarkAsDeleted(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedByUserId = deletedByUserId;
    }


    private Customer() { } // EF Core

    private Customer(Guid id, string ownerName, string companyName, string ownerPhone,
        string ownerEmail, string? industry, Guid ownerUserId) : base(id)
    {
        OwnerName = ownerName;
        CompanyName = companyName;
        OwnerPhone = ownerPhone;
        OwnerEmail = ownerEmail;
        Industry = industry;
        OwnerUserId = ownerUserId;
        Status = CustomerStatus.Active;
    }

    public static Customer Register(string ownerName, string companyName, string ownerPhone,
        string ownerEmail, string? industry, Guid ownerUserId)
    {
        var customer = new Customer(Guid.NewGuid(), ownerName, companyName, ownerPhone, ownerEmail, industry, ownerUserId);
        customer.RaiseDomainEvent(
        new CustomerRegisteredEvent(customer.Id, ownerUserId, ownerName, ownerEmail ,DateTime.UtcNow));
        return customer;
    }

    public void UpdateProfile(string ownerName, string companyName, string ownerPhone, string? industry)
    {
        OwnerName = ownerName;
        CompanyName = companyName;
        OwnerPhone = ownerPhone;
        Industry = industry;
        // OwnerEmail عمدًا مش هنا — تغييره محتاج Flow منفصل بيلمس Identity (زي ما اتفقنا)
    }

    public void Suspend()
    {
        if (Status == CustomerStatus.Suspended) return;
        Status = CustomerStatus.Suspended;
        RaiseDomainEvent(new CustomerStatusChangedEvent(Id, Status, DateTime.UtcNow));
    }

    public void Activate()
    {
        if (Status == CustomerStatus.Active) return;
        Status = CustomerStatus.Active;
        RaiseDomainEvent(new CustomerStatusChangedEvent(Id, Status, DateTime.UtcNow));
    }

    public void TransferOwnership(Guid newOwnerUserId, string newOwnerName, string newOwnerEmail)
    {
        if (newOwnerUserId == OwnerUserId) return;
        var previousOwner = OwnerUserId;
        OwnerUserId = newOwnerUserId;
        OwnerName = newOwnerName;
        OwnerEmail = newOwnerEmail;
        RaiseDomainEvent(new CustomerOwnershipTransferredEvent(Id, previousOwner, newOwnerUserId, DateTime.UtcNow));
    }

    public bool IsOwnedBy(Guid userId) => OwnerUserId == userId;

    public void UpdateEmail(string email)
    {
        OwnerEmail = email.Trim();

        RaiseDomainEvent(
            new CustomerEmailChangedEvent(
                Id,
                OwnerUserId,
                OwnerEmail,
                DateTime.UtcNow));
    }
}
public enum CustomerStatus
{
    Active = 0,
    Suspended = 1
}
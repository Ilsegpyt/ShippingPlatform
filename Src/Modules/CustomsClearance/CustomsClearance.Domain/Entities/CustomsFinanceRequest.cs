using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Domain.Entities;

// Customs Finance state of one shipment (1:1, created on the first Approve / Exclude).
// Amounts are not stored yet: EGP / EUR / USD come later from accounting.
public class CustomsFinanceRequest
{
    public Guid ShipmentId { get; set; }

    public CustomsFinanceStatus Status { get; set; }

    public DateOnly? DeferredUntilDate { get; set; } // Cairo date; Deferred only

    public string? Note { get; set; }

    public DateTime? ApprovedAtUtc { get; set; }
    public string? ApprovedByName { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
    public string UpdatedByName { get; set; } = null!;
}

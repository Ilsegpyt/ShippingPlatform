
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

/// <summary>
/// Persisted FCL candidate pairing an Import container with a selected Export container.
/// </summary>
public sealed class FclSuggestion
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ImportOperationId { get; private set; }
    public Operation ImportOperation { get; private set; } = null!;

    public Guid ImportContainerId { get; private set; }
    public OperationContainer ImportContainer { get; private set; } = null!;

    // The original Export operation and container from which the suggestion was generated.
    public Guid? ExportOperationId { get; private set; }
    public Operation? ExportOperation { get; private set; }

    // Nullable for compatibility with legacy suggestions.
    public Guid? ExportContainerId { get; private set; }
    public OperationContainer? ExportContainer { get; private set; }

    public Guid ClientId { get; private set; }
    public ClearanceType ClearanceType { get; private set; }
    public string BookingNumber { get; private set; } = null!;
    public Guid ShippingLineId { get; private set; }
    public ShippingLine ShippingLine { get; private set; } = null!;
    public Guid POLId { get; private set; }
    public Port POL { get; private set; } = null!;
    public Guid? POWId { get; private set; }
    public Port? POW { get; private set; }
    public DateOnly? CutOffDate { get; private set; }

    public string ContainerNumber { get; private set; } = null!;
    public ContainerType ContainerType { get; private set; }
    public string? Destination { get; private set; }
    public DateOnly RequiredLoadingDate { get; private set; }
    public TimeOnly RequiredLoadingTime { get; private set; }

    public bool FreeTimeMatch { get; private set; }
    public bool ShippingLineMatch { get; private set; }
    public bool PodPolMatch { get; private set; }
    public bool OffloadingLoadingTimeMatch { get; private set; }
    public bool ContainerSizeMatch { get; private set; }
    public bool DestinationMatch { get; private set; }

    public int MatchCount { get; private set; }
    public FclSuggestionStatus Status { get; private set; } = FclSuggestionStatus.Pending;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private FclSuggestion() { }

    public FclSuggestion(
        Guid importOperationId,
        Guid importContainerId,
        Guid exportOperationId,
        Guid exportContainerId,
        Guid clientId,
        ClearanceType clearanceType,
        string bookingNumber,
        Guid shippingLineId,
        Guid polId,
        Guid? powId,
        DateOnly? cutOffDate,
        string containerNumber,
        ContainerType containerType,
        string? destination,
        DateOnly requiredLoadingDate,
        TimeOnly requiredLoadingTime,
        bool freeTimeMatch,
        bool shippingLineMatch,
        bool podPolMatch,
        bool offloadingLoadingTimeMatch,
        bool containerSizeMatch,
        bool destinationMatch)
    {
        if (importOperationId == Guid.Empty ||
            importContainerId == Guid.Empty)
        {
            throw new ArgumentException(
                "The Import operation and container are required.");
        }

        if (exportOperationId == Guid.Empty ||
            exportContainerId == Guid.Empty)
        {
            throw new ArgumentException(
                "The selected Export operation and container are required.");
        }

        if (clientId == Guid.Empty ||
            shippingLineId == Guid.Empty ||
            polId == Guid.Empty)
        {
            throw new ArgumentException(
                "Client, shipping line and POL are required.");
        }

        if (string.IsNullOrWhiteSpace(bookingNumber))
            throw new ArgumentException("Booking number is required.", nameof(bookingNumber));

        if (string.IsNullOrWhiteSpace(containerNumber))
            throw new ArgumentException("Container number is required.", nameof(containerNumber));

        ImportOperationId = importOperationId;
        ImportContainerId = importContainerId;
        ExportOperationId = exportOperationId;
        ExportContainerId = exportContainerId;

        ClientId = clientId;
        ClearanceType = clearanceType;
        BookingNumber = bookingNumber.Trim();
        ShippingLineId = shippingLineId;
        POLId = polId;
        POWId = powId;
        CutOffDate = cutOffDate;

        ContainerNumber = containerNumber.Trim();
        ContainerType = containerType;
        Destination = string.IsNullOrWhiteSpace(destination)
            ? null
            : destination.Trim();

        RequiredLoadingDate = requiredLoadingDate;
        RequiredLoadingTime = requiredLoadingTime;

        FreeTimeMatch = freeTimeMatch;
        ShippingLineMatch = shippingLineMatch;
        PodPolMatch = podPolMatch;
        OffloadingLoadingTimeMatch = offloadingLoadingTimeMatch;
        ContainerSizeMatch = containerSizeMatch;
        DestinationMatch = destinationMatch;

        MatchCount = new[]
        {
            freeTimeMatch,
            shippingLineMatch,
            podPolMatch,
            offloadingLoadingTimeMatch,
            containerSizeMatch,
            destinationMatch
        }.Count(x => x);

        if (MatchCount < 5)
        {
            throw new ArgumentException(
                "Only FCL suggestions matching at least five of six criteria can be persisted.");
        }
    }

    public void MarkApproved()
    {
        if (Status == FclSuggestionStatus.Approved)
        {
            throw new InvalidOperationException(
                "This FCL suggestion has already been approved.");
        }

        if (MatchCount != 6)
        {
            throw new InvalidOperationException(
                "Only an exact 6/6 FCL match can be approved.");
        }

        Status = FclSuggestionStatus.Approved;
    }
}

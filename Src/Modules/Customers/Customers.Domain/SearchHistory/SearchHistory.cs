using BuildingBlocks.Domain;

namespace Customers.Domain.SearchHistory;

public sealed class SearchHistory : Entity<Guid>
{
    public Guid CustomerId { get; private set; }

    public string Origin { get; private set; } = null!;
    public string Destination { get; private set; } = null!;

    public string ContainerSize { get; private set; } = null!;

    public DateOnly DepartureDate { get; private set; }

    public int RoutesFound { get; private set; }

    public DateTime SearchedOnUtc { get; private set; }

    private SearchHistory()
    {
    }

    private SearchHistory(
        Guid id,
        Guid customerId,
        string origin,
        string destination,
        string containerSize,
        DateOnly departureDate,
        int routesFound,
        DateTime searchedOnUtc)
        : base(id)
    {
        CustomerId = customerId;
        Origin = origin;
        Destination = destination;
        ContainerSize = containerSize;
        DepartureDate = departureDate;
        RoutesFound = routesFound;
        SearchedOnUtc = searchedOnUtc;
    }

    public static SearchHistory Create(
        Guid customerId,
        string origin,
        string destination,
        string containerSize,
        DateOnly departureDate,
        int routesFound)
    {
        return new SearchHistory(
            Guid.NewGuid(),
            customerId,
            origin,
            destination,
            containerSize,
            departureDate,
            routesFound,
            DateTime.UtcNow);
    }
}
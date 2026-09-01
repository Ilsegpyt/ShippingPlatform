
using BuildingBlocks.Domain;

namespace Schedules.Domain.Schedule;

public sealed class Schedule : Entity<Guid>
{
    public string? RouteId { get; private set; }

    public ScheduleMode Mode { get; private set; }

    public DateOnly DepartureDate { get; private set; }

    public string Vessel { get; private set; } = null!;

    public string Origin { get; private set; } = null!;
    public string DeparturePortCode { get; private set; } = null!;
    public string DepartureCountry { get; private set; } = null!;

    public string Destination { get; private set; } = null!;
    public string ArrivalPortCode { get; private set; } = null!;
    public string ArrivalCountry { get; private set; } = null!;

    public string Carrier { get; private set; } = null!;
    public string CarrierCode { get; private set; } = null!;
    public string VoyageNumber { get; private set; } = null!;

    public DateOnly Arrival { get; private set; }

    public TimeSpan TransitTime { get; private set; }

    public DateOnly CutoffDate { get; private set; }
    public DateOnly PortCutoffDate { get; private set; }

    public string RateCurrency { get; private set; } = null!;
    public ContainerSize ContainerSize { get; private set; }
    public decimal RateAmount { get; private set; }
    public string? RateRemarks { get; private set; }

    public DateOnly ValidityDate { get; private set; }

    public int FreeTimeAtPOD { get; private set; }
    public int FreeTimeAtPOL { get; private set; }

    public string? TransshipmentData { get; private set; }
    public string? Notes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public static Schedule Create(
        string? routeId,
        ScheduleMode mode,
        DateOnly departureDate,
        string vessel,
        string origin,
        string departurePortCode,
        string departureCountry,
        string destination,
        string arrivalPortCode,
        string arrivalCountry,
        string carrier,
        string carrierCode,
        string voyageNumber,
        DateOnly arrival,
        TimeSpan transitTime,
        DateOnly cutoffDate,
        string rateCurrency,
        ContainerSize containerSize,
        decimal rateAmount,
        string? rateRemarks,
        DateOnly validityDate,
        int freeTimeAtPOD,
        int freeTimeAtPOL,
        string? transshipmentData,
        string? notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vessel);
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        ArgumentException.ThrowIfNullOrWhiteSpace(departurePortCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(departureCountry);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        ArgumentException.ThrowIfNullOrWhiteSpace(arrivalPortCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(arrivalCountry);
        ArgumentException.ThrowIfNullOrWhiteSpace(carrier);
        ArgumentException.ThrowIfNullOrWhiteSpace(carrierCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(voyageNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(rateCurrency);

        if (arrival < departureDate)
            throw new ArgumentException(
                "Arrival date cannot be before departure date.");

        if (cutoffDate >= departureDate)
            throw new ArgumentException(
                "Cut-off date must be before departure date.");

        if (transitTime < TimeSpan.Zero)
            throw new ArgumentException(
                "Transit time cannot be negative.");

        if (rateAmount < 0)
            throw new ArgumentException(
                "Rate amount cannot be negative.");

        if (freeTimeAtPOD < 0 || freeTimeAtPOL < 0)
            throw new ArgumentException(
                "Free time cannot be negative.");

        return new Schedule(
            Guid.NewGuid(),
            routeId,
            mode,
            departureDate,
            vessel,
            origin,
            departurePortCode,
            departureCountry,
            destination,
            arrivalPortCode,
            arrivalCountry,
            carrier,
            carrierCode,
            voyageNumber,
            arrival,
            transitTime,
            cutoffDate,
            rateCurrency,
            containerSize,
            rateAmount,
            rateRemarks,
            validityDate,
            freeTimeAtPOD,
            freeTimeAtPOL,
            transshipmentData,
            notes);
    }

    public void Update(
        string? routeId,
        ScheduleMode mode,
        DateOnly departureDate,
        string vessel,
        string origin,
        string departurePortCode,
        string departureCountry,
        string destination,
        string arrivalPortCode,
        string arrivalCountry,
        string carrier,
        string carrierCode,
        string voyageNumber,
        DateOnly arrival,
        TimeSpan transitTime,
        DateOnly cutoffDate,
        string rateCurrency,
        ContainerSize containerSize,
        decimal rateAmount,
        string? rateRemarks,
        DateOnly validityDate,
        int freeTimeAtPOD,
        int freeTimeAtPOL,
        string? transshipmentData,
        string? notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vessel);
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        ArgumentException.ThrowIfNullOrWhiteSpace(departurePortCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(departureCountry);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        ArgumentException.ThrowIfNullOrWhiteSpace(arrivalPortCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(arrivalCountry);
        ArgumentException.ThrowIfNullOrWhiteSpace(carrier);
        ArgumentException.ThrowIfNullOrWhiteSpace(carrierCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(voyageNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(rateCurrency);

        if (arrival < departureDate)
            throw new ArgumentException(
                "Arrival date cannot be before departure date.");

        if (cutoffDate >= departureDate)
            throw new ArgumentException(
                "Cut-off date must be before departure date.");

        if (transitTime < TimeSpan.Zero)
            throw new ArgumentException(
                "Transit time cannot be negative.");

        if (rateAmount < 0)
            throw new ArgumentException(
                "Rate amount cannot be negative.");

        if (freeTimeAtPOD < 0 || freeTimeAtPOL < 0)
            throw new ArgumentException(
                "Free time cannot be negative.");

        RouteId = routeId;
        Mode = mode;
        DepartureDate = departureDate;
        Vessel = vessel;

        Origin = origin;
        DeparturePortCode = departurePortCode;
        DepartureCountry = departureCountry;

        Destination = destination;
        ArrivalPortCode = arrivalPortCode;
        ArrivalCountry = arrivalCountry;

        Carrier = carrier;
        CarrierCode = carrierCode;
        VoyageNumber = voyageNumber;

        Arrival = arrival;
        TransitTime = transitTime;

        CutoffDate = cutoffDate;
        PortCutoffDate = departureDate.AddDays(-4);

        RateCurrency = rateCurrency;
        ContainerSize = containerSize;
        RateAmount = rateAmount;
        RateRemarks = rateRemarks;

        ValidityDate = validityDate;

        FreeTimeAtPOD = freeTimeAtPOD;
        FreeTimeAtPOL = freeTimeAtPOL;

        TransshipmentData = transshipmentData;
        Notes = notes;

        UpdatedAtUtc = DateTime.UtcNow;
    }
    public void Patch(
    string? routeId,
    ScheduleMode? mode,
    DateOnly? departureDate,
    string? vessel,
    string? origin,
    string? departurePortCode,
    string? departureCountry,
    string? destination,
    string? arrivalPortCode,
    string? arrivalCountry,
    string? carrier,
    string? carrierCode,
    string? voyageNumber,
    DateOnly? arrival,
    TimeSpan? transitTime,
    DateOnly? cutoffDate,
    string? rateCurrency,
    ContainerSize? containerSize,
    decimal? rateAmount,
    string? rateRemarks,
    DateOnly? validityDate,
    int? freeTimeAtPOD,
    int? freeTimeAtPOL,
    string? transshipmentData,
    string? notes)
    {
        if (departureDate.HasValue)
            DepartureDate = departureDate.Value;

        if (arrival.HasValue)
            Arrival = arrival.Value;

        if (Arrival < DepartureDate)
            throw new ArgumentException(
                "Arrival date cannot be before departure date.");

        if (cutoffDate.HasValue)
            CutoffDate = cutoffDate.Value;

        if (CutoffDate >= DepartureDate)
            throw new ArgumentException(
                "Cut-off date must be before departure date.");

        if (transitTime.HasValue)
        {
            if (transitTime.Value < TimeSpan.Zero)
                throw new ArgumentException(
                    "Transit time cannot be negative.");

            TransitTime = transitTime.Value;
        }

        if (rateAmount.HasValue)
        {
            if (rateAmount.Value < 0)
                throw new ArgumentException(
                    "Rate amount cannot be negative.");

            RateAmount = rateAmount.Value;
        }

        if (freeTimeAtPOD.HasValue)
        {
            if (freeTimeAtPOD.Value < 0)
                throw new ArgumentException(
                    "Free time at POD cannot be negative.");

            FreeTimeAtPOD = freeTimeAtPOD.Value;
        }

        if (freeTimeAtPOL.HasValue)
        {
            if (freeTimeAtPOL.Value < 0)
                throw new ArgumentException(
                    "Free time at POL cannot be negative.");

            FreeTimeAtPOL = freeTimeAtPOL.Value;
        }

        if (routeId is not null)
            RouteId = routeId;

        if (mode.HasValue)
            Mode = mode.Value;

        if (vessel is not null)
            Vessel = vessel;

        if (origin is not null)
            Origin = origin;

        if (departurePortCode is not null)
            DeparturePortCode = departurePortCode;

        if (departureCountry is not null)
            DepartureCountry = departureCountry;

        if (destination is not null)
            Destination = destination;

        if (arrivalPortCode is not null)
            ArrivalPortCode = arrivalPortCode;

        if (arrivalCountry is not null)
            ArrivalCountry = arrivalCountry;

        if (carrier is not null)
            Carrier = carrier;

        if (carrierCode is not null)
            CarrierCode = carrierCode;

        if (voyageNumber is not null)
            VoyageNumber = voyageNumber;

        if (rateCurrency is not null)
            RateCurrency = rateCurrency;

        if (containerSize.HasValue)
            ContainerSize = containerSize.Value;

        if (rateRemarks is not null)
            RateRemarks = rateRemarks;

        if (validityDate.HasValue)
            ValidityDate = validityDate.Value;

        if (transshipmentData is not null)
            TransshipmentData = transshipmentData;

        if (notes is not null)
            Notes = notes;

        PortCutoffDate = DepartureDate.AddDays(-4);

        UpdatedAtUtc = DateTime.UtcNow;
    }
    private Schedule()
    {
    }

    private Schedule(
        Guid id,
        string? routeId,
        ScheduleMode mode,
        DateOnly departureDate,
        string vessel,
        string origin,
        string departurePortCode,
        string departureCountry,
        string destination,
        string arrivalPortCode,
        string arrivalCountry,
        string carrier,
        string carrierCode,
        string voyageNumber,
        DateOnly arrival,
        TimeSpan transitTime,
        DateOnly cutoffDate,
        string rateCurrency,
        ContainerSize containerSize,
        decimal rateAmount,
        string? rateRemarks,
        DateOnly validityDate,
        int freeTimeAtPOD,
        int freeTimeAtPOL,
        string? transshipmentData,
        string? notes)
        : base(id)
    {
        RouteId = routeId;
        Mode = mode;
        DepartureDate = departureDate;
        Vessel = vessel;

        Origin = origin;
        DeparturePortCode = departurePortCode;
        DepartureCountry = departureCountry;

        Destination = destination;
        ArrivalPortCode = arrivalPortCode;
        ArrivalCountry = arrivalCountry;

        Carrier = carrier;
        CarrierCode = carrierCode;
        VoyageNumber = voyageNumber;

        Arrival = arrival;
        TransitTime = transitTime;

        CutoffDate = cutoffDate;
        PortCutoffDate = departureDate.AddDays(-4);

        RateCurrency = rateCurrency;
        ContainerSize = containerSize;
        RateAmount = rateAmount;
        RateRemarks = rateRemarks;

        ValidityDate = validityDate;

        FreeTimeAtPOD = freeTimeAtPOD;
        FreeTimeAtPOL = freeTimeAtPOL;

        TransshipmentData = transshipmentData;
        Notes = notes;

        CreatedAtUtc = DateTime.UtcNow;
    }
}

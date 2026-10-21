using System.Globalization;
using System.Reflection;
using Customers.Contracts;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.CreateOperation;
using Operations.Application.Operations.UpdateExport;
using Operations.Application.Operations.UpdateImport;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.ImportTrips;

public static class ImportTripsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/imports", async (
            ImportTripsRequest request,
            ISender sender,
            IOperationRepository repository,
            IOperationsUnitOfWork unitOfWork,
            ICustomerQueries customerQueries,
            CancellationToken ct) =>
        {
            if (request.Trips is null || request.Trips.Count == 0)
                return Results.BadRequest(new { message = "The upload contains no trips.", errors = new[] { "No trip rows were supplied." } });

            var errors = new List<string>();
            var customers = await customerQueries.GetAllForOperationsAsync(ct);
            var shippingLines = await repository.GetActiveShippingLinesAsync(ct);
            var ports = await repository.GetActivePortsAsync(ct);
            var transporters = await repository.GetActiveTransportersAsync(ct);
            var drivers = await repository.GetActiveDriversAsync(ct);
            var vehicles = await repository.GetActiveVehiclesAsync(ct);

            var groups = request.Trips
                .Select((trip, index) => new { Trip = trip, Index = index })
                .GroupBy(x => new { x.Trip.Direction, Group = x.Trip.Group ?? string.Empty })
                .ToList();

            // Resolve all names and required values before opening the transaction.
            var prepared = new List<PreparedOperation>();
            foreach (var group in groups)
            {
                var rows = group.Select(x => x.Trip).ToList();
                var first = rows[0];
                var direction = ParseDirection(first.Direction);
                if (direction is null)
                {
                    errors.Add($"Group '{group.Key.Group}': direction must be Import or Export.");
                    continue;
                }
                if (rows.Any(x => !string.Equals(x.Direction, first.Direction, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Add($"Group '{group.Key.Group}': mixed directions are not allowed.");
                    continue;
                }

                string? Get(ImportTrip row, string key) => row.Fields.TryGetValue(key, out var value) ? Clean(value) : null;
                var clientName = Get(first, "client");
                var client = FindByText(customers, clientName, "CompanyName", "companyName", "CustomerName", "customerName");
                var clientId = ReadGuid(client, "CustomerId", "customerId", "Id", "id");
                var lineName = Get(first, "line");
                var line = FindByName(shippingLines, lineName, x => x.Name);
                var lineId = line?.Id;
                if (clientId is null) errors.Add($"Group '{group.Key.Group}': customer '{clientName ?? "(empty)"}' was not found.");
                if (lineId is null) errors.Add($"Group '{group.Key.Group}': active shipping line '{lineName ?? "(empty)"}' was not found.");

                var containerCount = rows.Count;
                if (TryInt(Get(first, "vol"), out var declaredCount) && declaredCount != containerCount)
                    errors.Add($"Group '{group.Key.Group}': CNTR Volume is {declaredCount}, but {containerCount} container rows were supplied.");

                if (direction == OperationType.Import)
                {
                    var podName = Get(first, "pod");
                    var pod = FindPort(ports, podName);
                    var free = ParseDate(Get(first, "free"));
                    var reqDate = ParseDate(Get(first, "reqDate"));
                    var reqTime = ParseTime(Get(first, "reqTime"));
                    if (pod is null) errors.Add($"Group '{group.Key.Group}': active POD '{podName ?? "(empty)"}' was not found.");
                    if (free is null) errors.Add($"Group '{group.Key.Group}': Free Time Till is missing or invalid.");
                    if (reqDate is null) errors.Add($"Group '{group.Key.Group}': Required Offloading Date is missing or invalid.");
                    if (reqTime is null) errors.Add($"Group '{group.Key.Group}': Required Offloading Time is missing or invalid.");
                    if (string.IsNullOrWhiteSpace(Get(first, "dest"))) errors.Add($"Group '{group.Key.Group}': Destination is required.");
                    prepared.Add(new PreparedOperation(group.Key.Group, direction.Value, rows, clientId, lineId,
                        Get(first, "bl"), Get(first, "dest"), pod?.Id, free, reqDate, reqTime,
                        null, null, null, null, null, null, shippingLines, ports, transporters, drivers, vehicles));
                }
                else
                {
                    var polName = Get(first, "pol");
                    var pol = FindPort(ports, polName);
                    var powName = Get(first, "pow");
                    var pow = string.IsNullOrWhiteSpace(powName) ? null : FindPort(ports, powName);
                    var clearance = ParseClearance(Get(first, "clrType"));
                    var cut = ParseDate(Get(first, "cut"));
                    if (pol is null) errors.Add($"Group '{group.Key.Group}': active POL '{polName ?? "(empty)"}' was not found.");
                    if (!string.IsNullOrWhiteSpace(powName) && pow is null) errors.Add($"Group '{group.Key.Group}': active POW '{powName}' was not found.");
                    if (clearance is null) errors.Add($"Group '{group.Key.Group}': Clearance Type must be CERT or BOSLA.");
                    if (string.IsNullOrWhiteSpace(Get(first, "booking"))) errors.Add($"Group '{group.Key.Group}': Booking Number is required.");
                    if (clearance == ClearanceType.Cert && !string.IsNullOrWhiteSpace(Get(first, "customs"))) errors.Add($"Group '{group.Key.Group}': Customs must be empty for CERT.");
                    if (clearance == ClearanceType.Bosla && !string.IsNullOrWhiteSpace(Get(first, "cert"))) errors.Add($"Group '{group.Key.Group}': Certificate Number must be empty for BOSLA.");
                    prepared.Add(new PreparedOperation(group.Key.Group, direction.Value, rows, clientId, lineId,
                        null, null, null, null, null, null,
                        clearance, Get(first, "booking"), pol?.Id, pow?.Id, cut, Get(first, "dest"), shippingLines, ports, transporters, drivers, vehicles));
                }
            }

            // Resolve per-container references and unsupported container sizes before any write.
            foreach (var item in prepared)
            {
                foreach (var row in item.Rows)
                {
                    string? Get(string key) => row.Fields.TryGetValue(key, out var value) ? Clean(value) : null;
                    if (ParseContainerType(Get("size")) is null)
                        errors.Add($"Row {row.Row?.ToString() ?? "?"}, group '{item.Group}', container '{Get("cntr") ?? "(empty)"}': unsupported container size/type '{Get("size") ?? "(empty)"}'. Use a supported container type or 20'/40' dry standard size.");
                    if (!string.IsNullOrWhiteSpace(Get("trucker")) && FindByName(transporters, Get("trucker"), x => x.Name) is null)
                        errors.Add($"Row {row.Row?.ToString() ?? "?"}, group '{item.Group}': active transporter '{Get("trucker")}' was not found.");
                    if (!string.IsNullOrWhiteSpace(Get("car")) && !vehicles.Any(x => Same(x.CarNumber, Get("car"))))
                        errors.Add($"Row {row.Row?.ToString() ?? "?"}, group '{item.Group}': active vehicle/car '{Get("car")}' was not found.");
                    if ((!string.IsNullOrWhiteSpace(Get("mob")) || !string.IsNullOrWhiteSpace(Get("driver"))) && FindDriver(drivers, Get("mob"), Get("driver")) is null)
                        errors.Add($"Row {row.Row?.ToString() ?? "?"}, group '{item.Group}': active driver '{Get("driver") ?? Get("mob")}' was not found.");
                }
            }

            if (errors.Count > 0)
                return Results.BadRequest(new { message = "The entire TPT upload was rejected. No operations were saved.", errors });

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                foreach (var item in prepared)
                {
                    var first = item.Rows[0];
                    string? Get(ImportTrip row, string key) => row.Fields.TryGetValue(key, out var value) ? Clean(value) : null;
                    var create = new CreateOperationCommand(
                        item.ClientId!.Value, item.ShippingLineId!.Value, item.Direction, item.Rows.Count,
                        item.Direction == OperationType.Import ? item.MblNumber : null,
                        item.Direction == OperationType.Import ? item.Destination : null,
                        item.PodId, item.FreeTime, item.RequiredDate, item.RequiredTime,
                        item.ClearanceType, item.BookingNumber, item.PolId, item.PowId, item.CutOffDate,
                        item.Direction == OperationType.Export ? item.ExportDestination : null);
                    var created = await sender.Send(create, ct);
                    if (!created.IsSuccess)
                        throw new ImportRejectedException($"Group '{item.Group}': {created.Error}");

                    var operationId = created.Value;
                    var operation = await repository.GetByIdForUpdateAsync(operationId, ct)
                        ?? throw new ImportRejectedException($"Group '{item.Group}': newly created operation could not be reloaded.");
                    operation.MarkAsImported();
                    await unitOfWork.SaveChangesAsync(ct);

                    var containers = operation.Containers.OrderBy(x => x.Ordinal).ToList();
                    if (containers.Count != item.Rows.Count)
                        throw new ImportRejectedException($"Group '{item.Group}': container count mismatch after creation.");

                    if (item.Direction == OperationType.Import)
                    {
                        var containerItems = item.Rows.Select((row, index) =>
                        {
                            string? Get(string key) => row.Fields.TryGetValue(key, out var value) ? Clean(value) : null;
                            var transporter = FindByName(transporters, Get("trucker"), x => x.Name);
                            var vehicle = vehicles.FirstOrDefault(x => Same(x.CarNumber, Get("car")));
                            var driver = FindDriver(drivers, Get("mob"), Get("driver"));
                            return new UpdateImportContainerItem(containers[index].Id,
                                ParseDate(Get("arrDate")), ParseTime(Get("arrTime")),
                                ParseDate(Get("gateOut")), ParseTime(Get("gateOutT")), ParseContainerType(Get("size")),
                                Get("cntr"), transporter?.Id, vehicle?.Id, driver?.Id,
                                ParseDateTime(Get("checkout")), Get("missDoc"), ParseDate(Get("missRcv")),
                                Get("dest"), ParseDate(Get("free")));
                        }).ToList();
                        var update = await sender.Send(new UpdateImportCommand(operationId, Get(first, "shipNo"), Get(first, "cert"), ParseDateTime(Get(first, "invRcv")), containerItems), ct);
                        if (!update.IsSuccess) throw new ImportRejectedException($"Group '{item.Group}': {update.Error}");
                    }
                    else
                    {
                        var containerItems = item.Rows.Select((row, index) =>
                        {
                            string? Get(string key) => row.Fields.TryGetValue(key, out var value) ? Clean(value) : null;
                            return new UpdateExportContainerItem(containers[index].Id, ParseContainerType(Get("size")), Get("cntr"),
                                ParseDate(Get("reqDate")), ParseTime(Get("reqTime")), Get("dest"));
                        }).ToList();
                        var update = await sender.Send(new UpdateExportCommand(operationId, Get(first, "shipNo"), Get(first, "cert"), Get(first, "customs"), containerItems), ct);
                        if (!update.IsSuccess) throw new ImportRejectedException($"Group '{item.Group}': {update.Error}");
                    }
                }

                await unitOfWork.CommitTransactionAsync(ct);
                return Results.Ok(new { version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), importedRows = request.Trips.Count, importedOperations = prepared.Count });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                var message = ex is ImportRejectedException ? ex.Message : "TPT upload failed. No operations were saved.";
                return Results.BadRequest(new { message, errors = new[] { message } });
            }
        }).RequirePermission(PermissionCatalog.OperationsCreate);
    }

    private static OperationType? ParseDirection(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "import" => OperationType.Import,
        "export" => OperationType.Export,
        _ => null
    };

    private static ClearanceType? ParseClearance(string? value) => value?.Trim().ToUpperInvariant() switch
    {
        "CERT" => ClearanceType.Cert,
        "BOSLA" or "BOSLA/BL" or "B/L" => ClearanceType.Bosla,
        _ => null
    };

    private static ContainerType? ParseContainerType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var text = value.Trim().ToUpperInvariant();
        if (text.Contains("LCL")) return null;
        if (text.Contains("45")) return ContainerType.DryHigh45;
        if (text.Contains("20")) return ContainerType.DryStandard20;
        if (text.Contains("40")) return ContainerType.DryStandard40;
        return Enum.TryParse<ContainerType>(value, true, out var parsed) ? parsed : null;
    }

    private static DateOnly? ParseDate(string? value) => DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null;
    private static TimeOnly? ParseTime(string? value) => TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null;
    private static DateTime? ParseDateTime(string? value) => DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result) ? result : ParseDate(value)?.ToDateTime(TimeOnly.MinValue);
    private static bool TryInt(string? value, out int result) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) || value.Trim() is "-" or "." or ".." ? null : value.Trim();
    private static bool Same(string? left, string? right) => !string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right) && string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);

    private static T? FindByName<T>(IEnumerable<T> source, string? name, Func<T, string> selector) where T : class =>
        string.IsNullOrWhiteSpace(name) ? null : source.FirstOrDefault(x => Same(selector(x), name));

    private static Port? FindPort(IEnumerable<Port> ports, string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : ports.FirstOrDefault(x => Same(x.Name, value) || Same(x.Code, value));

    private static Driver? FindDriver(IEnumerable<Driver> drivers, string? mobile, string? name) =>
        drivers.FirstOrDefault(x => !string.IsNullOrWhiteSpace(mobile) && Same(x.MobileNumber, mobile)) ??
        drivers.FirstOrDefault(x => !string.IsNullOrWhiteSpace(name) && Same(x.Name, name));

    private static object? FindByText<T>(IEnumerable<T> items, string? value, params string[] properties) where T : class
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return items.Cast<object>().FirstOrDefault(item => properties.Any(p => Same(ReadString(item, p), value)));
    }

    private static string? ReadString(object? instance, params string[] names)
    {
        if (instance is null) return null;
        foreach (var name in names)
        {
            var property = instance.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var value = property?.GetValue(instance)?.ToString();
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }
        return null;
    }

    private static Guid? ReadGuid(object? instance, params string[] names)
    {
        var value = ReadString(instance, names);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private sealed record PreparedOperation(
        string Group, OperationType Direction, List<ImportTrip> Rows, Guid? ClientId, Guid? ShippingLineId,
        string? MblNumber, string? Destination, Guid? PodId, DateOnly? FreeTime, DateOnly? RequiredDate, TimeOnly? RequiredTime,
        ClearanceType? ClearanceType, string? BookingNumber, Guid? PolId, Guid? PowId, DateOnly? CutOffDate, string? ExportDestination,
        IReadOnlyList<ShippingLine> ShippingLines, IReadOnlyList<Port> Ports, IReadOnlyList<Transporter> Transporters,
        IReadOnlyList<Driver> Drivers, IReadOnlyList<Vehicle> Vehicles);

    private sealed class ImportRejectedException(string message) : Exception(message);
}

public sealed record ImportTripsRequest(string? RequestId, IReadOnlyList<ImportTrip> Trips);
public sealed record ImportTrip(string Direction, string? Group, Dictionary<string, string?> Fields, int? Row = null);
